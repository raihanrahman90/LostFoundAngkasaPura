    using LostFound.Authorize;
using LostFoundAngkasaPura.Controllers.User;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.DTO.Notification;
using LostFoundAngkasaPura.Service.Admin;
using LostFoundAngkasaPura.Service.AdminNotification;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminNotificationService _adminNotificationService;

        public AdminController(ILogger<AuthController> logger, IAdminService adminService, IAdminNotificationService adminNotificationService)
        {
            _adminService = adminService;
            _adminNotificationService = adminNotificationService;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<AdminResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize]
        public async Task<IActionResult> GetListAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? email = null,
            [FromQuery] string? access = null,
            [FromQuery] string? name = null)
        {
            var result = await _adminService.GetListAdmin(page, size, email, access, name);
            return new OkObjectResult(new DefaultResponse<Pagination<AdminResponseDTO>>(result));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(DefaultResponse<AdminResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> CreateAdmin(
            [FromBody] AdminCreateRequestDTO request)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminService.CreateAdmin(request, userId);
            return new OkObjectResult(new DefaultResponse<AdminResponseDTO>(result));
        }

        [HttpGet("{adminId}")]
        [ProducesResponseType(typeof(DefaultResponse<AdminResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> GetDetailAdmin(
            [FromRoute] string adminId)
        {
            var result = await _adminService.GetDetailAdmin(adminId);
            return new OkObjectResult(new DefaultResponse<AdminResponseDTO>(result));
        }

        [HttpDelete("{adminId}")]
        [ProducesResponseType(typeof(DefaultResponse<AdminResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> DeactivateAdmin(
            [FromRoute] string adminId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminService.DeactivateAdmin(adminId, userId);
            return new OkObjectResult(new DefaultResponse<AdminResponseDTO>(result));
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AdminAccessResponseDTO), 200)]
        public async Task<ActionResult> Login([FromBody] AdminLoginRequestDTO dto)
        {
            var result = await _adminService.Login(dto);
            return new OkObjectResult(new DefaultResponse<AdminAccessResponseDTO>(result));
        }

        [HttpGet("access-token")]
        [ProducesResponseType(typeof(AdminAccessResponseDTO), 200)]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            var result = await _adminService.GetAccessToken(refreshToken);
            return new OkObjectResult(new DefaultResponse<AdminAccessResponseDTO>(result));
        }

        [HttpGet("profile")]
        [CustomAuthorize(true)]
        public async Task<IActionResult> GetProfile()
        {
            var adminId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminService.GetProfile(adminId);
            return new OkObjectResult(new DefaultResponse<ProfileResponseDTO>(result));
        }

        [HttpPost("profile")]
        [ProducesResponseType(typeof(AdminAccessResponseDTO), 200)]
        [CustomAuthorize(true)]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateRequestDTO request)
        {
            var adminId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminService.UpdateProfile(request, adminId);
            return new OkObjectResult(new DefaultResponse<ProfileResponseDTO>(result));
        }

        [HttpGet("logout-all")]
        [CustomAuthorize(true)]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            await _adminService.LogoutAll(userId);
            return new OkObjectResult(new DefaultResponse<bool>(true));
        }

        [HttpGet("notification/count")]
        [CustomAuthorize(true)]
        public async Task<IActionResult> GetNotificationCount()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var count = await _adminNotificationService.CountNotification(userId);
            return new OkObjectResult(new DefaultResponse<int>(count));
        }

        [HttpGet("notification")]
        [CustomAuthorize(true)]
        public async Task<IActionResult> GetNotification()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminNotificationService.GetListNotification(userId);
            return new OkObjectResult(new DefaultResponse<List<NotificationResponse>>(result));
        }

        [HttpPost("{adminId}/reset-password")]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> ResetPassword([FromRoute] string adminId)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _adminService.ResetPassword(adminId, userId);
            return new OkObjectResult(new DefaultResponse<string>(result));
        }
    }
}
