using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Customer;
using LostFoundAngkasaPura.Service.User;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/User")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetListCustomer(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? name = "")
        {
            var result = await _userService.GetListCustomer(page, size, name);
            return new OkObjectResult(new DefaultResponse<Pagination<CustomerResponseDTO>>(result));
        }

        [HttpGet("{userId}")]
        [CustomAuthorize]
        public async Task<IActionResult> GetDetailCustomer(
            [FromRoute] string userId)
        {
            var result = await _userService.GetDetailCustomer(userId);
            return new OkObjectResult(new DefaultResponse<CustomerResponseDTO>(result));
        }
    }
}
