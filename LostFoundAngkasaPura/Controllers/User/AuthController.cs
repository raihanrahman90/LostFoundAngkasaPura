using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.DTO.Notification;
using LostFoundAngkasaPura.Service.Auth;
using LostFoundAngkasaPura.Service.UserNotification;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.User
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserNotificationService _userNotificationService;

        public AuthController(
            ILogger<AuthController> logger, 
            IAuthService authService,
            IUserNotificationService userNotificationService)
        {
            _authService = authService;
            _userNotificationService = userNotificationService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _authService.Login(dto);
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                });
            return new OkObjectResult(new DefaultResponse<string>(result.AccessToken));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            var result = await _authService.Register(dto);
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
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var result = await _authService.GetAccessToken(refreshToken);
            return new OkObjectResult(new DefaultResponse<string>(result.AccessToken));
        }

        [HttpGet("logout")]
        [CustomAuthorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Append("refreshToken", "",
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddDays(-1)
                });
            return new OkObjectResult(new DefaultResponse<string>(""));
        }

        [HttpGet("logout-all")]
        [CustomAuthorize]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var response = await _authService.Logout(userId);
            return new OkObjectResult(response);
        }

        [HttpGet("notification")]
        [CustomAuthorize]
        public async Task<IActionResult> GetListNotification()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _userNotificationService.GetListNotification(userId);
            return new OkObjectResult(new DefaultResponse<List<NotificationResponse>>(result));
        }

        [HttpGet("notification/count")]
        [CustomAuthorize]
        public async Task<IActionResult> GetCountNotification()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var count = await _userNotificationService.CountNotification(userId);
            return new OkObjectResult(new DefaultResponse<int>(count));
        }

        [HttpGet("access-token/check")]
        [CustomAuthorize]
        public async Task<IActionResult> CheckAccessToken()
        {
            return Ok();
        }
    }
}
