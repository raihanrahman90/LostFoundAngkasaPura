using LostFoundAngkasaPura.DTO.Dashboard;

namespace LostFoundAngkasaPura.Service.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardData(DateTime? startDate, DateTime? endDate);
        Task<DashboardGrafikData> GetGrafikData(DateTime? startDate, DateTime? endDate);
        Task<byte[]> DownloadToExcel(DateTime? startDate, DateTime? endDate);
        Task<DashboardGrafikData> GetGrafikRating(DateTime? startDate, DateTime? endDate, string? type);
        Task<byte[]> GrafikToExcel(DateTime? startDate, DateTime? endDate, string? type);
    }
}
