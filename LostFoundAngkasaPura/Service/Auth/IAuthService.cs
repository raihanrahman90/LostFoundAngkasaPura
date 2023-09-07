using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.DTO.ForgotPassword;

namespace LostFoundAngkasaPura.Service.Auth
{
    public interface IAuthService
    {
        Task<AccessResponseDTO> Register(RegisterRequestDTO request);
        Task<AccessResponseDTO> Login(LoginRequestDTO request);
        Task<AccessResponseDTO> GetAccessToken(string refreshToken);
        Task<DefaultResponse<UserResponseDTO>> Logout(string userId);
        Task ForgotPasswordRequestCode(ForgotPasswordCodeRequestDTO request);
        Task ForgotPasswordResetPassword(ForgotPasswordResetPasswordRequestDTO request);
        Task<UserResponseDTO> GetData(string userId);
        Task<UserResponseDTO> UpdateData(UserDataUpdateRequestDTO request, string userId);

        Task<bool> DeleteData(string userId);
    }
}
