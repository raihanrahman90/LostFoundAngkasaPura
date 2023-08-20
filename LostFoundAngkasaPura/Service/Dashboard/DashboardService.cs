using ClosedXML.Excel;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.Dashboard;
using LostFoundAngkasaPura.DTO.Error;
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

        public async Task<DashboardData> GetDashboardData(DateTime? startDateRequest, DateTime? endDateRequest)
        {
            DateTime? startDate = startDateRequest == null ? null : getFirstDay(startDateRequest.Value.Year, startDateRequest.Value.Month);
            DateTime? endDate = endDateRequest == null ? null : getLastDay(endDateRequest.Value.Year, endDateRequest.Value.Month);
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
                (startDate == null || t.CreatedDate >= startDate) &&
                (endDate == null || t.CreatedDate <= endDate)).CountAsync();

            return new DashboardData()
            {
               ClosedCount  =countClosed,
               FoundCount = countClosed,
               WaitingCount = countWaiting,
               ClaimCount = countClaim
            };

        }

        public async Task<DashboardGrafikData> GetGrafikData(DateTime? startDate, DateTime? endDate)
        {
            DateTime startFilter, endFilter;
            if(startDate == null && endDate == null)
            {
                startFilter = DateTime.Now;
                endFilter = await _unitOfWork.ItemFoundRepository.OrderBy(t => t.FoundDate).Select(t => t.FoundDate).FirstOrDefaultAsync();
            }
            else if ((startDate == null && endDate != null) || (endDate == null && startDate != null)) 
                throw new DataMessageError(ErrorMessageConstant.PleaseFillBothDate);
            else
            {
                startFilter = (DateTime)startDate;
                endFilter = (DateTime)endDate;
            }
            startFilter = getFirstDay(startFilter.Year, startFilter.Month);
            endFilter = getLastDay(endFilter.Year, endFilter.Month);
            var labels = new List<string>();
            for(int i = startFilter.Year; i <= endFilter.Year; i++)
            {
                var startMonth = startFilter.Year == i ? startFilter.Month : 1;
                var endMonth = endFilter.Year == i ? endFilter.Month : 12;
                for(int j = startMonth; j <= endMonth; j++)
                {
                    labels.Add($"{j}-{i}");
                }
            }
            var dataComplete = await _unitOfWork.ItemFoundRepository.Where(t =>
                    t.Status.ToLower().Equals(ItemFoundStatus.Closed.ToLower()) &&
                    t.ActiveFlag)
                .GroupBy(t=>new { Month= t.FoundDate.Month, Year = t.FoundDate.Year})
                .Select(t=>new {Month=t.Key.Month, Year=t.Key.Year, Count=t.Count()})
                .ToDictionaryAsync(t=>$"{t.Month}-{t.Year}", t=>t.Count);
            var dataWaiting = await _unitOfWork.ItemFoundRepository.Where(t =>
                    t.Status.ToLower().Equals(ItemFoundStatus.Closed.ToLower()) &&
                    t.ActiveFlag &&
                    t.FoundDate >= startFilter &&
                    t.FoundDate <= endFilter)
                .GroupBy(t => new { Month = t.FoundDate.Month, Year = t.FoundDate.Year })
                .Select(t => new { Month = t.Key.Month, Year = t.Key.Year, Count = t.Count() })
                .ToDictionaryAsync(t => $"{t.Month}-{t.Year}", t => t.Count);
            var dataFound = await _unitOfWork.ItemFoundRepository
                .Where(t =>
                    t.Status.ToLower().Equals(ItemFoundStatus.Closed.ToLower()) &&
                    t.ActiveFlag &&
                    t.FoundDate >= startFilter &&
                    t.FoundDate <= endFilter)
                .GroupBy(t => new { Month = t.FoundDate.Month, Year = t.FoundDate.Year })
                .Select(t => new { Month = t.Key.Month, Year = t.Key.Year, Count = t.Count() })
                .ToDictionaryAsync(t => $"{t.Month}-{t.Year}", t => t.Count);
            var dataClaim = await _unitOfWork.ItemClaimRepository.Where(t=>
                    t.ActiveFlag &&
                    t.CreatedDate >= startFilter &&
                    t.CreatedDate <= endFilter)
                .GroupBy(t => new { Month = t.CreatedDate.Value.Month, Year = t.CreatedDate.Value.Year })
                .Select(t => new { Month = t.Key.Month, Year = t.Key.Year, Count = t.Count() })
                .ToDictionaryAsync(t => $"{t.Month}-{t.Year}", t => t.Count);
            var datasetComplete = new DashboardDataset()
            {
                Label = "Closed",
                Data = labels.Select(t => dataComplete.ContainsKey(t) ? dataComplete[t]:0).ToList(),
                BorderColor = "#3498db",
                BackgroundColor = "#3498db"
            };
            var datasetWaiting = new DashboardDataset()
            {
                Label = "Waiting",
                Data = labels.Select(t => dataWaiting.ContainsKey(t) ? dataWaiting[t]:0).ToList(),
                BorderColor = "#f1c40f",
                BackgroundColor = "#f1c40f"
            };
            var datasetFound = new DashboardDataset()
            {
                Label = "Found",
                Data = labels.Select(t => dataFound.ContainsKey(t) ? dataFound[t] : 0).ToList(),
                BorderColor= "#34495e",
                BackgroundColor= "#34495e"
            };
            var datasetClaim = new DashboardDataset()
            {
                Label = "Claim",
                Data = labels.Select(t => dataClaim.ContainsKey(t) ? dataClaim[t] : 0).ToList(),
                BorderColor= "#3498db",
                BackgroundColor= "#3498db"
            };
            return new DashboardGrafikData()
            {
                Labels = labels,
                Datasets = new List<DashboardDataset>()
                {
                    datasetComplete, datasetWaiting, datasetFound, datasetClaim
                }
            };
        }

        public async Task<byte[]> DownloadToExcel(DateTime? startDate, DateTime? endDate)
        {
            var data = await GetGrafikData(startDate, endDate);
            var dataClaim = data.Datasets.Where(t => t.Label.Equals("Claim")).FirstOrDefault().Data;
            var dataFound = data.Datasets.Where(t => t.Label.Equals("Found")).FirstOrDefault().Data;
            var dataClosed = data.Datasets.Where(t => t.Label.Equals("Closed")).FirstOrDefault().Data;
            var tanggal = data.Labels;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");
                worksheet.Cell("A1").Value = "Tanggal";
                worksheet.Cell("B1").Value = "Item Found";
                worksheet.Cell("C1").Value = "Item Closed";
                worksheet.Cell("D1").Value = "Claim Count";
                for(var i = 0; i < tanggal.Count; i++)
                {
                    worksheet.Cell($"A{i+2}").Value = tanggal[i];
                    worksheet.Cell($"B{i+2}").Value = dataFound[i];
                    worksheet.Cell($"C{i+2}").Value = dataClosed[i];
                    worksheet.Cell($"D{i+2}").Value = dataClaim[i];
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private DateTime getLastDay(int year, int month)
        {
            return new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
        }

        private DateTime getFirstDay(int year, int month)
        {
            return new DateTime(year, month, 1, 0, 0, 0);
        }
    }
}
