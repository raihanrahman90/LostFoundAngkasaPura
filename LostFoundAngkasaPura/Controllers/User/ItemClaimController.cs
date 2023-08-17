using LostFound.Authorize;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemCategory;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemComment;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.ItemClaim;
using LostFoundAngkasaPura.Service.ItemComment;
using LostFoundAngkasaPura.Service.ItemFound;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Controllers.User
{
    [Route("Item-Claim")]
    [ApiController]
    public class ItemClaimController : ControllerBase
    {
        private readonly IItemClaimService _itemClaim;
        private readonly IItemCommentService _itemComment;

        public ItemClaimController(IItemClaimService itemClaim, IItemCommentService itemComment)
        {
            _itemClaim = itemClaim;
            _itemComment = itemComment;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<ItemClaimResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize]
        public async Task<IActionResult> GetListItemFound(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.GetListItemClaim(page, size, false,userId);
            return new OkObjectResult(new DefaultResponse<Pagination<ItemClaimResponseDTO>>(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DefaultResponse<ItemClaimResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ClaimItem(
            [FromBody] ItemClaimRequestDTO request)
        {

            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.ClaimItem(request, userId);
            return new OkObjectResult(new DefaultResponse<ItemClaimResponseDTO>(result));
        }
        [HttpGet("{itemClaimId}")]
        [ProducesResponseType(typeof(DefaultResponse<ItemClaimResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize]
        public async Task<IActionResult> GetDetailItemClaim(
            [FromRoute] string itemClaimId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.GetItemClaimDetail(itemClaimId);
            return new OkObjectResult(new DefaultResponse<ItemClaimResponseDTO>(result));
        }

        [HttpGet("{itemClaimId}/comment")]
        [ProducesResponseType(typeof(DefaultResponse<ItemCommentResponseDTO>), 200)]
        [CustomAuthorize]
        public async Task<IActionResult> GetListComment(
            [FromRoute] string itemClaimId)
        {
            var result = await _itemComment.GetComment(itemClaimId);
            return new OkObjectResult(new DefaultResponse<List<ItemCommentResponseDTO>>(result));
        }

        [HttpPost("{itemClaimId}/comment")]
        [ProducesResponseType(typeof(DefaultResponse<ItemCommentResponseDTO>), 200)]
        [CustomAuthorize]
        public async Task<IActionResult> CreateComment(
            [FromRoute] string itemClaimId,
            [FromBody] ItemCommentCreateRequestDTO request)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemComment.AddComment(request, userId, null);
            return new OkObjectResult(new DefaultResponse<ItemCommentResponseDTO>(result));
        }
    }
}
