using LostFound.Authorize;
using LostFoundAngkasaPura.Controllers.User;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Admin;
using LostFoundAngkasaPura.DTO.Dashboard;
using LostFoundAngkasaPura.Service.Dashboard;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

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
        public async Task<IActionResult> GetDataDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _dashboardService.GetDashboardData(startDate, endDate);
            return new OkObjectResult(new DefaultResponse<DashboardData>(result));
        }

        [HttpGet("grafik")]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<AdminResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true)]
        public async Task<IActionResult> GetGrafikDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _dashboardService.GetGrafikData(startDate, endDate);
            return new OkObjectResult(new DefaultResponse<DashboardGrafikData>(result));
        }

        [HttpGet("download")]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<AdminResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> DowloadToExcel(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var content = await _dashboardService.DownloadToExcel(startDate, endDate);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Opportunities.xlsx");
        }

    }
}
