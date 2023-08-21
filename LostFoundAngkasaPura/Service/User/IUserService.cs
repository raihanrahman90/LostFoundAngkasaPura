using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Customer;

namespace LostFoundAngkasaPura.Service.User
{
    public interface IUserService
    {
        Task<Pagination<UserResponseDTO>> GetListCustomer(int page, int size, string name);

        Task<UserResponseDTO> GetDetailCustomer(string userId);
    }
}
