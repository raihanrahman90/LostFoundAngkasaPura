using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.Service.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LostFound.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _authService.Login(dto);
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken);
            return new OkObjectResult(new DefaultResponse<string>() { StatusCode = 200, Success = true, Data = result.AccessToken });
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            var result = await _authService.Register(dto);
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken);
            return new OkObjectResult(new DefaultResponse<string>() { StatusCode = 200, Success = true, Data = result.AccessToken });
        }

        [HttpGet("access-token")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var result = await _authService.GetAccessToken(refreshToken);
            return new OkObjectResult(new DefaultResponse<string>() { StatusCode = 200, Success = true, Data = result.AccessToken });
        }

        [HttpGet("logout")]
        [CustomAuthorize("false")]
        public IActionResult Logout()
        {

            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            _authService.Logout(userId);
            HttpContext.Response.Cookies.Delete("refreshToken");
            return new OkObjectResult(new DefaultResponse<string>() {StatusCode= 200, Success=true, Data=null });
        }
    }
}
