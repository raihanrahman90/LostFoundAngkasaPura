using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemComment;
using LostFoundAngkasaPura.Service.ItemComment;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Item-Comment")]
    [ApiController]
    public class AdminItemCommentController : ControllerBase
    {

        private readonly IItemCommentService _itemComment;

        public AdminItemCommentController(IItemCommentService itemComment)
        {
            _itemComment = itemComment;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ItemCommentResponseDTO>), 200)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> GetListComment([FromQuery] string itemClaimId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemComment.GetComment(itemClaimId);
            return new OkObjectResult(new DefaultResponse<List<ItemCommentResponseDTO>>(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DefaultResponse<ItemCommentResponseDTO>), 200)]
        [CustomAuthorize(false, false)]
        public async Task<IActionResult> CreateComment([FromBody] ItemCommentCreateRequestDTO request)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemComment.AddComment(request, null, userId);
            return new OkObjectResult(new DefaultResponse<ItemCommentResponseDTO>(result));
        }

    }
}
