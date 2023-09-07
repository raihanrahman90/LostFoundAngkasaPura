using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Auth;
using LostFoundAngkasaPura.DTO.ForgotPassword;
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
            return new OkObjectResult(new DefaultResponse<AccessResponseDTO>(result));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            var result = await _authService.Register(dto);
            return new OkObjectResult(new DefaultResponse<AccessResponseDTO>(result));
        }

        [HttpGet("access-token")]
        [ProducesResponseType(typeof(AccessResponseDTO), 200)]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            var result = await _authService.GetAccessToken(refreshToken);
            return new OkObjectResult(new DefaultResponse<AccessResponseDTO>(result));
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

        [HttpPost("forgot-password/code")]
        public async Task<IActionResult> ForgotPasswordCode([FromBody] ForgotPasswordCodeRequestDTO request)
        {
            await _authService.ForgotPasswordRequestCode(request);
            return Ok();
        }

        [HttpPost("forgot-password/reset-password")]
        public async Task<IActionResult> ForgotPasswordResetPassword([FromBody] ForgotPasswordResetPasswordRequestDTO request )
        {
            await _authService.ForgotPasswordResetPassword(request);
            return Ok();
        }

        [HttpGet("my-data")]
        [CustomAuthorize]
        public async Task<IActionResult> GetMyData()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var data = await _authService.GetData(userId);
            return new OkObjectResult(new DefaultResponse<UserResponseDTO>(data));
        }

        [HttpPost("my-data")]
        [CustomAuthorize]
        public async Task<IActionResult> UpdateMyData([FromBody] UserDataUpdateRequestDTO request)
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var data = await _authService.UpdateData(request, userId);
            return new OkObjectResult(new DefaultResponse<UserResponseDTO>(data));
        }

        [HttpDelete("my-data")]
        [CustomAuthorize]
        public async Task<IActionResult> DeleteData()
        {
            var userId = User.Claims.Where(t => t.Type.Equals("Id")).FirstOrDefault().Value;
            var result = await _authService.DeleteData(userId);
            return new OkObjectResult(new DefaultResponse<bool>(result));
        }

    }
}
