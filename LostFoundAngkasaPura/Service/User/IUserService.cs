using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Customer;

namespace LostFoundAngkasaPura.Service.User
{
    public interface IUserService
    {
        Task<Pagination<CustomerResponseDTO>> GetListCustomer(int page, int size, string name);

        Task<CustomerResponseDTO> GetDetailCustomer(string userId);

        Task<DAL.Model.User> GetUserById(string userId);
    }
}
