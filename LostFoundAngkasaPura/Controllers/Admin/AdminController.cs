    using LostFound.Authorize;
using LostFoundAngkasaPura.Controllers.User;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;
using LostFoundAngkasaPura.Service.Admin;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(ILogger<AuthController> logger, IAdminService adminService)
        {
            _adminService = adminService;
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
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                });
            return new OkObjectResult(new DefaultResponse<string>(result.AccessToken));
        }

        [HttpGet("access-token")]
        [ProducesResponseType(typeof(AdminAccessResponseDTO), 200)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var result = await _adminService.GetAccessToken(refreshToken);
            return new OkObjectResult(new DefaultResponse<string>(result.AccessToken));
        }

        [HttpGet("logout")]
        [CustomAuthorize(true)]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            return new OkObjectResult(new DefaultResponse<string>(""));
        }

        [HttpGet("logout-all")]
        [CustomAuthorize(true)]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            await _adminService.LogoutAll(userId);
            HttpContext.Response.Cookies.Delete("refreshToken");
            return new OkObjectResult(new DefaultResponse<bool>(true));
        }

    }
}
