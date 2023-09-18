using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;

namespace LostFoundAngkasaPura.Service.Admin
{
    public interface IAdminService
    {
        Task<AdminAccessResponseDTO> Login(AdminLoginRequestDTO request);
        Task<AdminAccessResponseDTO> GetAccessToken(string refreshToken);
        Task<Pagination<AdminResponseDTO>> GetListAdmin(int page, int size, string? email, string? access, string? name);
        Task<AdminResponseDTO> CreateAdmin(AdminCreateRequestDTO request, string adminId);
        Task<AdminResponseDTO> GetDetailAdmin(string adminId);
        Task<AdminResponseDTO> UpdateAdmin(AdminCreateRequestDTO request, string adminUpdateId, string adminId);
        Task<AdminResponseDTO> DeactivateAdmin(string adminId, string userId);
        Task LogoutAll(string userId);
        Task<ProfileResponseDTO> GetProfile(string adminId);
        Task<ProfileResponseDTO> UpdateProfile(ProfileUpdateRequestDTO request, string adminId);
        Task<string> ResetPassword(string adminId, string userId);
        Task<DAL.Model.Admin> GetAdminById(string adminId);
        Task testLogger();
    }
}
