using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.Service.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LostFound.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(ILogger<AuthController> logger, IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _authService.Login(dto);
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
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
                    Secure = true,
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
        [CustomAuthorize("false")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            return new OkObjectResult(new DefaultResponse<string>(""));
        }

        [HttpGet("logout-all")]
        [CustomAuthorize("false")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var response = await _authService.Logout(userId);
            return new OkObjectResult(response);
        }

    }
}
