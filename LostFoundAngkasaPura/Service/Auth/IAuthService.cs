using LostFoundAngkasaPura.DTO.Auth;

namespace LostFoundAngkasaPura.Service.Auth
{
    public interface IAuthService
    {
        Task<AccessResponseDTO> Register(RegisterRequestDTO request);
    }
}
