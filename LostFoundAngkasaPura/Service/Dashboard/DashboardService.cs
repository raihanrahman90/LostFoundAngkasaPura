using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.Dashboard;
using Microsoft.EntityFrameworkCore;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<DashboardData> GetDashboardData(DateTime? startDate, DateTime? endDate)
        {
            var countItemFound = await _unitOfWork.ItemFoundRepository
                .Where(t=>t.ActiveFlag && (startDate == null || t.FoundDate >= startDate) && (endDate == null || t.FoundDate <= endDate)).CountAsync();
            var countClosed = await _unitOfWork.ItemFoundRepository
                .Where(t => 
                    t.ActiveFlag && 
                    (startDate == null || t.FoundDate >= startDate) && 
                    (endDate == null || t.FoundDate <= endDate) &&
                    t.Status.ToLower().Equals(ItemFoundStatus.Closed))
                .CountAsync();
            var countWaiting = await _unitOfWork.ItemFoundRepository
                .Where(t =>
                    t.ActiveFlag &&
                    (startDate == null || t.FoundDate >= startDate) &&
                    (endDate == null || t.FoundDate <= endDate) &&
                    t.Status.ToLower().Equals(ItemFoundStatus.Confirmation))
                .CountAsync();
            var countClaim = await _unitOfWork.ItemClaimRepository
                .Where(t =>
                t.ActiveFlag &&
                (startDate == null || t.ClaimDate >= startDate) &&
                (endDate == null || t.ClaimDate <= endDate)).CountAsync();

            return new DashboardData()
            {
               ClosedCount  =countClosed,
               FoundCount = countClosed,
               WaitingCount = countWaiting,
               ClaimCount = countClaim
            };

        }
    }
}
