using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.Service.AdminNotification;
using LostFoundAngkasaPura.Service.ItemClaim;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Item-Claim")]
    [ApiController]
    public class ItemClaimController : ControllerBase
    {
        private readonly IItemClaimService _itemClaim;
        private readonly IAdminNotificationService _adminNotification;

        public ItemClaimController(
            IItemClaimService itemClaim, IAdminNotificationService adminNotification)
        {
            _itemClaim = itemClaim;
            _adminNotification = adminNotification;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<ItemClaimResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, false)]
        public async Task<IActionResult> GetListItemClaim(
            [FromQuery] string? itemFoundId=null,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? status = null)
        {
            var result = await _itemClaim.GetListItemClaimByItemFoundId(page, size, itemFoundId, status);
            return new OkObjectResult(new DefaultResponse<Pagination<ItemClaimResponseDTO>>(result));
        }

        [HttpGet("{itemClaimId}")]
        [ProducesResponseType(typeof(DefaultResponse<ItemClaimResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, false)]
        public async Task<IActionResult> GetDetailItemClaim(
            [FromRoute] string itemClaimId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.GetItemClaimDetail(itemClaimId);
            await _adminNotification.DeleteNotification(userId, itemClaimId);
            return new OkObjectResult(new DefaultResponse<ItemClaimResponseDTO>(result));
        }

        [HttpPost("{itemClaimId}/approve")]
        [ProducesResponseType(typeof(DefaultResponse<ItemClaimResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, false)]
        public async Task<IActionResult> ConfirmClaim(
            [FromBody] ItemClaimApproveRequestDTO request,
            [FromRoute] string itemClaimId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.ApproveClaim(request, itemClaimId, userId);
            return new OkObjectResult(new DefaultResponse<ItemClaimResponseDTO>(result));
        }

        [HttpPost("{itemClaimId}/reject")]
        [ProducesResponseType(typeof(DefaultResponse<ItemClaimResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, false)]
        public async Task<IActionResult> RejectClaim(
            [FromBody] ItemClaimRejectRequestDTO request,
            [FromRoute] string itemClaimId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _itemClaim.RejectClaim(request, itemClaimId, userId);
            return new OkObjectResult(new DefaultResponse<ItemClaimResponseDTO>(result));
        }

        [HttpGet("{itemClaimId}/report")]
        public async Task<IActionResult> GetReport(
            [FromRoute] string itemClaimId)
        {
            string result = await _itemClaim.DownloadAsPdf(itemClaimId);
            return new OkObjectResult(new DefaultResponse<string>(result));
        }
    }
}
