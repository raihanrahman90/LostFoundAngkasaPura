using AutoMapper;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.ItemCategory;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.ItemCategory
{
    public class ItemCategoryService : IItemCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ItemCategoryService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemCategory, ItemCategoryResponseDTO>();
            }));
        }

        public async Task<DAL.Model.ItemCategory> CreateCategory(string categoryName)
        {
            var category = await _unitOfWork.ItemCategoryRepository.FirstOrDefaultAsync(t => t.Category.ToLower().Equals(categoryName.ToLower()));
            if (category != null) return category;
            category = new DAL.Model.ItemCategory();
            category.Category = categoryName;
            await _unitOfWork.ItemCategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();
            return category;
        }

        public async Task<List<ItemCategoryResponseDTO>> GetListCategory()
        {
            var result = await _unitOfWork.ItemCategoryRepository.Where(t => t.ActiveFlag)
                .Select(t=>_mapper.Map<ItemCategoryResponseDTO>(t))
                .ToListAsync();
            return result;
        }
    }
}
