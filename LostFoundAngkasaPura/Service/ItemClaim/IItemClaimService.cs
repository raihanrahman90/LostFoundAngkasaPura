using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;

namespace LostFoundAngkasaPura.Service.ItemClaim
{
    public interface IItemClaimService
    {
        Task<Pagination<ItemClaimResponseDTO>> GetListItemClaim(int page, int size, bool isAdmin, string? userId);
        Task<Pagination<ItemClaimResponseDTO>> GetListItemClaimByItemFoundId(int page, int size, string itemFoundId);
        Task<ItemClaimResponseDTO> ClaimItem(ItemClaimRequestDTO request, string userId);
        Task<ItemClaimResponseDTO> ApproveClaim(ItemClaimApproveRequestDTO request, string itemClaimId, string userId);
        Task<ItemClaimResponseDTO> RejectClaim(ItemClaimRejectRequestDTO request, string itemClaimId, string userId);
    }
}
