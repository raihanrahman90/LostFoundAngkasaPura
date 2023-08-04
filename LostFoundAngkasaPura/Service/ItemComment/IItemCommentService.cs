using LostFoundAngkasaPura.DTO.ItemComment;

namespace LostFoundAngkasaPura.Service.ItemComment
{
    public interface IItemCommentService
    {
        Task<List<ItemCommentResponseDTO>> GetComment(string itemClaimId);
        Task<ItemCommentResponseDTO> AddComment(ItemCommentCreateRequestDTO request, string? userId, string? adminId);
    }
}
