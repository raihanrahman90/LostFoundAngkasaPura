using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemFound;

namespace LostFoundAngkasaPura.Service.ItemFound
{
    public interface IItemFoundService
    {
        Task<Pagination<ItemFoundResponseDTO>> GetListItemFound(int page, int size, string? name, string? category, string? status, DateTime? foundDate);
        Task<ItemFoundResponseDTO> GetDetailItemFound(string itemFoundId);
        Task<ItemFoundResponseDTO> CreateItemFound(ItemFoundCreateRequestDTO request, string adminId);
        Task<ItemFoundResponseDTO> UpdateItemFound(ItemFoundCreateRequestDTO request, string itemFoundId, string adminId);
        Task<ItemFoundResponseDTO> UpdateStatus(string status, string userId,DAL.Model.ItemFound itemFound);
        Task<ItemFoundResponseDTO> ClosedItem(string itemFoundId, string userId);
    }
}
