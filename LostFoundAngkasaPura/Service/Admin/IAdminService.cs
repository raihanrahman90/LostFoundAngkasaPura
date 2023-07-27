using LostFoundAngkasaPura.DTO.Admin;

namespace LostFoundAngkasaPura.Service.Admin
{
    public interface IAdminService
    {
        Task<AccessResponseDTO> Login(AdminLoginRequestDTO request);
    }
}
