using LostFound.Authorize;
using LostFoundAngkasaPura.Controllers.User;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;
using LostFoundAngkasaPura.DTO.Dashboard;
using LostFoundAngkasaPura.Service.Admin;
using LostFoundAngkasaPura.Service.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {

        private readonly IDashboardService _dashboardService;

        public AdminDashboardController(ILogger<AuthController> logger, IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<AdminResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true)]
        public async Task<IActionResult> GetListAdmin(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _dashboardService.GetDashboardData(startDate, endDate);
            return new OkObjectResult(new DefaultResponse<DashboardData>(result));
        }

    }
}
