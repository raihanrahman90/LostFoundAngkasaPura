using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;

namespace LostFoundAngkasaPura.Service.ItemClaim
{
    public interface IItemClaimService
    {
        Task<Pagination<ItemClaimResponseDTO>> GetListItemClaim(int page, int size, bool isAdmin, string? userId);
        Task<ItemClaimResponseDTO> ClaimItem(ItemClaimRequestDTO request, string userId);
        Task<ItemClaimResponseDTO> ConfirmClaim(ItemClaimRequestDTO request, string itemFoundId, string userId);
        Task<ItemClaimResponseDTO> RejectClaim(ItemClaimRequestDTO request, string itemFoundId, string userId);
    }
}
