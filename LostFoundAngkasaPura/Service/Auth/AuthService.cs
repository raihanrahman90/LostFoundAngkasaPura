using LostFoundAngkasaPura.DTO.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LostFound.DAL.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LostFound.DAL.Repositories;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly string JwtSecret;
        private readonly string ValidIssuer;
        private readonly string ValidAudience;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IConfiguration configuration, JwtSecurityTokenHandler jwtSecurityTokenHandler, IUnitOfWork unitOfWork)
        {
            JwtSecret = configuration.GetValue<string>("JWT:Secret");
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _unitOfWork = unitOfWork;
         }

        public async Task<AccessResponseDTO> Register(RegisterRequestDTO request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var refreshToken = await GetUniqueToken();
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

            var accessToken = GetToken(request.Email, user.Id);
            return new AccessResponseDTO()
            {
                AccessToken = accessToken,
                RefeshToken = refreshToken
            };
        }

        private string GetToken(string email, string userId)
        {
            var authClaims = new List<Claim>()
            {
                new Claim("Id", userId),
                new Claim("Email", email),
                new Claim("IsAdmin", "false"),
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

        private async Task<string> GetUniqueToken()
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
    }
}
