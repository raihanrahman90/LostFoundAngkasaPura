using LostFoundAngkasaPura.DTO.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LostFoundAngkasaPura.DAL.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LostFoundAngkasaPura.DAL.Repositories;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using LostFoundAngkasaPura.DTO.Error;
using AutoMapper;
using LostFoundAngkasaPura.DTO;

namespace LostFoundAngkasaPura.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly string JwtSecret;
        private readonly string ValidIssuer;
        private readonly string ValidAudience;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AuthService(IConfiguration configuration, JwtSecurityTokenHandler jwtSecurityTokenHandler, IUnitOfWork unitOfWork)
        {
            JwtSecret = configuration.GetValue<string>("JWT:Secret");
            ValidIssuer = configuration.GetValue<string>("JWT:ValidIssuer");
            ValidAudience = configuration.GetValue<string>("JWT:ValidAudience");
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _unitOfWork = unitOfWork;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<User, UserResponseDTO>();
            }));
         }

        public async Task<AccessResponseDTO> GetAccessToken(string refreshToken)
        {
            var user = await _unitOfWork.UserRepository.Where(t => t.RefreshToken.Equals(refreshToken)).FirstOrDefaultAsync();
            if (user == null) throw new DataMessageError(ErrorMessageConstant.RefreshTokenNotFound);

            var accessToken = GetAccessToken(user.Email, user.Id);
            return new AccessResponseDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AccessResponseDTO> Login(LoginRequestDTO request)
        {
            var user = await _unitOfWork.UserRepository.Where(t => t.Email.Equals(request.Email) && t.ActiveFlag).FirstOrDefaultAsync();
            if (user == null) throw new DataMessageError(ErrorMessageConstant.EmailNotFound);

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid) throw new DataMessageError(ErrorMessageConstant.PasswordWrong);

            if (String.IsNullOrWhiteSpace(user.RefreshToken))
            {
                var refreshToken = await GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveAsync();
            }

            var accessToken = GetAccessToken(user.Email, user.Id);
            return new AccessResponseDTO()
            {
                RefreshToken = user.RefreshToken,
                AccessToken = accessToken
            };
        }

        public async Task<AccessResponseDTO> Register(RegisterRequestDTO request)
        {
            var isEmailUsed = await _unitOfWork.UserRepository.AnyAsync(t => t.Email.Equals(request.Email) && t.ActiveFlag);
            if (isEmailUsed) throw new DataMessageError(ErrorMessageConstant.EmailAlreadyExist);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var refreshToken = await GenerateRefreshToken();
            var user = new User()
            {
                Email = request.Email,
                Password = hashedPassword,
                Name = request.Name,
                Phone = request.Phone,
                RefreshToken = refreshToken
            };
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            var accessToken = GetAccessToken(request.Email, user.Id);
            return new AccessResponseDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GetAccessToken(string email, string userId)
        {
            var authClaims = new List<Claim>()
            {
                new Claim("Id", userId),
                new Claim("Email", email),
                new Claim("IsAdmin", "false"),
                new Claim("Access", "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            var expire = DateTime.Now.AddHours(1);
            var token = new JwtSecurityToken(
                issuer: ValidIssuer,
                audience: ValidAudience,
                expires: expire,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                
                );
            var result = _jwtSecurityTokenHandler.WriteToken(token);
            return result;
        }

        public async Task<DefaultResponse<UserResponseDTO>> Logout(string userId)
        {
            var user = await findUserById(userId);
            user.RefreshToken = null;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();
            return new DefaultResponse<UserResponseDTO>(_mapper.Map<UserResponseDTO>(user));
        }

        private async Task<string> GenerateRefreshToken()
        {
            string token = "";
            while (true)
            {
                // token is a cryptographically strong random sequence of values
                token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                // ensure token is unique by checking against db
                var isTokenExist = await _unitOfWork.UserRepository.AnyAsync(t => t.RefreshToken.Equals(token));
                if (!isTokenExist) break;
            }
            return token;
        }

        private async Task<User> findUserById(string userId)
        {
            var user = await _unitOfWork.UserRepository.Where(t => t.Id.Equals(userId) && t.ActiveFlag).FirstOrDefaultAsync();
            if (user == null) throw new DataMessageError(ErrorMessageConstant.UserNotFound);
            return user;
        }
    }
}
