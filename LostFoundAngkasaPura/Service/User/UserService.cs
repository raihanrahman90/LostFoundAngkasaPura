using AutoMapper;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Customer;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.User
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.User, CustomerResponseDTO>();
            }));
        }

        public async Task<CustomerResponseDTO> GetDetailCustomer(string userId)
        {
            var data = await _unitOfWork.UserRepository
                            .Where(t => t.Id.Equals(userId))
                            .Select(t => _mapper.Map<CustomerResponseDTO>(t))
                            .FirstOrDefaultAsync();
            return data;
        }

        public async Task<Pagination<CustomerResponseDTO>> GetListCustomer(int page, int size, string name)
        {
            var query = _unitOfWork.UserRepository
                                .Where(t => t.ActiveFlag && t.Name.Contains(name));
            var count = await query.CountAsync();
            var data = await query
                .OrderBy(t=>t.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(t => _mapper.Map<CustomerResponseDTO>(t)).ToListAsync();
            return new Pagination<CustomerResponseDTO>(data, count, size, page);
        }
    }
}
