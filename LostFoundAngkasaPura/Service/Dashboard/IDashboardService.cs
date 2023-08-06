using LostFoundAngkasaPura.DTO.Dashboard;

namespace LostFoundAngkasaPura.Service.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardData(DateTime? startDate, DateTime? endDate);
    }
}
