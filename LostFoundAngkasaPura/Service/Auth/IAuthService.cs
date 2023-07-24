using LostFoundAngkasaPura.DTO.Auth;

namespace LostFoundAngkasaPura.Service.Auth
{
    public interface IAuthService
    {
        Task<AccessResponseDTO> Register(RegisterRequestDTO request);
        Task<AccessResponseDTO> Login(LoginRequestDTO request);
        Task<AccessResponseDTO> GetAccessToken(string accessToken);
        Task Logout(string userId);
    }
}
