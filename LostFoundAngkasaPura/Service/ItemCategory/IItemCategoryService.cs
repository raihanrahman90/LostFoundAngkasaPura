using LostFoundAngkasaPura.DTO.ItemCategory;

namespace LostFoundAngkasaPura.Service.ItemCategory
{
    public interface IItemCategoryService
    {
        Task<List<ItemCategoryResponseDTO>> GetListCategory();
        Task<DAL.Model.ItemCategory> CreateCategory(string category);
    }
}
