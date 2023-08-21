using AutoMapper;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.Service.Mailer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string JwtSecret;
        private readonly string ValidIssuer;
        private readonly string ValidAudience;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IMapper _mapper;
        private readonly IMailerService _mailserService;

        public AdminService(
            IUnitOfWork uow, 
            JwtSecurityTokenHandler jwtSecurityTokenHandler, 
            IConfiguration configuration,
            IMailerService mailerService)
        {
            ValidIssuer = configuration.GetValue<string>("JWT:ValidIssuer");
            ValidAudience = configuration.GetValue<string>("JWT:ValidAudience");
            JwtSecret = configuration.GetValue<string>("JWT:Secret");
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _mailserService = mailerService;
            _unitOfWork = uow;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.Admin, AdminResponseDTO>();
            }));
        }

        public async Task<AdminAccessResponseDTO> GetAccessToken(string refreshToken)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.RefreshToken.Equals(refreshToken) && t.ActiveFlag).FirstOrDefaultAsync();
            if (admin == null) throw new DataMessageError(ErrorMessageConstant.RefreshTokenNotFound);

            var accessToken = GenerateAccessToken(admin.Email, admin.Id, admin.Access);
            return new AdminAccessResponseDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<Pagination<AdminResponseDTO>> GetListAdmin(int page, int size, string? email, string? access, string? name)
        {
            var query = _unitOfWork.AdminRepository
                                .Where(t =>
                                (email == null || t.Email.Contains(email)) &&
                                (access == null || t.Access.Equals(access)) &&
                                (name == null || t.Name.Contains(name)) &&
                                t.ActiveFlag);
            var count = await query.CountAsync();
            var list = await query
                                .Skip((page - 1) * size)
                                .Take(size)
                                .Select(t => _mapper.Map<AdminResponseDTO>(t))
                                .ToListAsync();
            return new Pagination<AdminResponseDTO>(list, count, size, page);
        }

        public async Task<AdminResponseDTO> GetDetailAdmin(string adminId)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Id.Equals(adminId)).FirstOrDefaultAsync();
            if (admin == null) throw new NotFoundError();

            return _mapper.Map<AdminResponseDTO>(admin);
        }

        public async Task<AdminResponseDTO> CreateAdmin(AdminCreateRequestDTO request, string adminId)
        {
            bool isSuperAdmin = false;
            if (request.Access.ToLower().Equals(AdminAccess.SuperAdmin.ToLower())) isSuperAdmin = true;
            else if (request.Access.ToLower().Equals(AdminAccess.Admin.ToLower())) isSuperAdmin = false;
            else throw new DataMessageError(ErrorMessageConstant.NotValidField("Access"));

            bool isEmailUsed = await _unitOfWork.AdminRepository.Where(t => t.ActiveFlag && t.Email.ToLower().Equals(request.Email.ToLower())).AnyAsync();
            if (isEmailUsed) throw new DataMessageError(ErrorMessageConstant.EmailAlreadyExist);

            var password = Utils.GeneralUtils.GetRandomPassword(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var refreshToken = await GenerateRefreshToken();
            var admin = new DAL.Model.Admin()
            {
                Access = isSuperAdmin?AdminAccess.SuperAdmin:AdminAccess.Admin,
                CreatedBy = adminId,
                CreatedDate = DateTime.Now,
                Email = request.Email,
                LastUpdatedBy = adminId,
                LastUpdatedDate = DateTime.Now,
                Password = hashedPassword,
                Name = request.Name,
                Unit = request.Unit,
                RefreshToken = refreshToken
            };

            await _unitOfWork.AdminRepository.AddAsync(admin);
            await _unitOfWork.SaveAsync();
            _mailserService.CreateAdmin(request.Email, request.Name, password);
            return _mapper.Map<AdminResponseDTO>(admin);
        }

        public async Task<AdminAccessResponseDTO> Login(AdminLoginRequestDTO request)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Email.ToLower().Equals(request.Email.ToLower()) && t.ActiveFlag).FirstOrDefaultAsync();
            if (admin == null) throw new DataMessageError(ErrorMessageConstant.EmailNotFound);

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, admin.Password);
            if (!isPasswordValid) throw new DataMessageError(ErrorMessageConstant.PasswordWrong);

            if (String.IsNullOrWhiteSpace(admin.RefreshToken))
            {
                var refreshToken = await GenerateRefreshToken();
                admin.RefreshToken = refreshToken;
                _unitOfWork.AdminRepository.Update(admin);
                await _unitOfWork.SaveAsync();
            }
            var accessToken = GenerateAccessToken(admin.Email, admin.Id, admin.Access);
            return new AdminAccessResponseDTO()
            {
                AccessToken = accessToken,
                RefreshToken = admin.RefreshToken
            };
        }

        public async Task LogoutAll(string userId)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Id.Equals(userId) && t.ActiveFlag).FirstOrDefaultAsync();
            if (admin == null) throw new NotFoundError();

            admin.RefreshToken = await GenerateRefreshToken();
            _unitOfWork.AdminRepository.Update(admin);
            await _unitOfWork.SaveAsync();
        }

        public async Task<AdminResponseDTO> DeactivateAdmin(string adminId, string userId)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Id.Equals(adminId) && t.ActiveFlag).FirstOrDefaultAsync();
            if (admin == null) throw new NotFoundError();
            admin.ActiveFlag = false;
            admin.LastUpdatedDate = DateTime.Now;
            admin.LastUpdatedBy = userId;
            _unitOfWork.AdminRepository.Update(admin);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<AdminResponseDTO>(admin);

        }

        private string GenerateAccessToken(string email, string userId, string access)
        {
            var authClaims = new List<Claim>()
            {
                new Claim("Id", userId),
                new Claim("Email", email),
                new Claim("IsAdmin", "true"),
                new Claim("Access", access),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            var token = new JwtSecurityToken(
                issuer: ValidIssuer,
                audience: ValidAudience,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)

                );
            var result = _jwtSecurityTokenHandler.WriteToken(token);
            return result;
        }

        private async Task<string> GenerateRefreshToken()
        {
            string token = "";
            while (true)
            {
                // token is a cryptographically strong random sequence of values
                token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                // ensure token is unique by checking against db
                var isTokenExist = await _unitOfWork.AdminRepository.AnyAsync(t => t.RefreshToken.Equals(token));
                if (!isTokenExist) break;
            }
            return token;
        }

        public async Task<ProfileResponseDTO> GetProfile(string adminId)
        {
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Id.Equals(adminId)).FirstOrDefaultAsync();
            return new ProfileResponseDTO()
            {
                Email = admin.Email
            };
        }

        public async Task<ProfileResponseDTO> UpdateProfile(ProfileUpdateRequestDTO request, string adminId)
        {
            var isEmailUsed = await _unitOfWork.AdminRepository.Where(t => t.Email.Equals(request.Email) && !t.Id.Equals(adminId)).AnyAsync();
            if (isEmailUsed) throw new DataMessageError(ErrorMessageConstant.EmailAlreadyExist);
            var admin = await _unitOfWork.AdminRepository.Where(t => t.Id.Equals(adminId)).FirstOrDefaultAsync();
            admin.Email = request.Email;
            if (request.UpdatePassword)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                admin.Password = hashedPassword;
                admin.RefreshToken = await GenerateRefreshToken();
            }
            _unitOfWork.AdminRepository.Update(admin);
            await _unitOfWork.SaveAsync();
            return new ProfileResponseDTO()
            {
                Email = request.Email
            };
        }
    }
}
