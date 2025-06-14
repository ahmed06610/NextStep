using Microsoft.EntityFrameworkCore;
using NextStep.Core.Const;
using NextStep.Core.DTOs.Report;
using NextStep.Core.Interfaces;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextStep.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int DelayedThresholdDays = 5;

        private readonly List<string> _arabicMonthNames = new List<string>
        {
            "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
            "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
        };

        private enum TimeGrouping { Day, Month, Year }

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Helper Methods

        private TimeGrouping GetTimeGrouping(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue) return TimeGrouping.Month;
            var duration = endDate.Value - startDate.Value;
            if (duration.TotalDays <= 31) return TimeGrouping.Day;
            if (duration.TotalDays <= 366) return TimeGrouping.Month;
            return TimeGrouping.Year;
        }

        private IQueryable<Application> GetFilteredApplications(DateTime? startDate, DateTime? endDate)
        {
            var query = _unitOfWork.Application.GetQueryable(null);
            if (startDate.HasValue) query = query.Where(a => a.CreatedDate >= startDate.Value.Date);
            if (endDate.HasValue) query = query.Where(a => a.CreatedDate < endDate.Value.AddDays(1).Date);
            return query.Include(a => a.Steps);
        }

        // Helper for stats that count EVERYTHING that ever passed through a department (Total, Approved, Rejected).
        private IQueryable<Application> GetHistoricalDepartmentApplications(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            return GetFilteredApplications(startDate, endDate).Include(c=>c.ApplicationType)
                .Where(a => a.ApplicationHistories.Any(h => h.ActionByDeptId == departmentId));
        }

        // Helper for stats that count only what is CURRENTLY in a department's inbox (Pending, Delayed).
        private IQueryable<Application> GetCurrentDepartmentInbox(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            var pendingStatus = AppStatues.قيد_التنفيذ.ToString();
            var query = GetFilteredApplications(startDate, endDate)
               .Where(a => a.Status == pendingStatus && a.Steps.DepartmentID == departmentId);
            return query;
        }

        #endregion

        #region General Reports (Fully Corrected)

        public async Task<GlobalStatsDTO> GetGlobalStatsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = GetFilteredApplications(startDate, endDate);
            var pendingStatus = AppStatues.قيد_التنفيذ.ToString();
            var approvedStatus = AppStatues.مقبول.ToString();
            var rejectedStatus = AppStatues.مرفوض.ToString();
            var delayedDate = DateTime.UtcNow.AddDays(-DelayedThresholdDays);
            return new GlobalStatsDTO
            {
                TotalRequests = await query.CountAsync(),
                ApprovedRequests = await query.CountAsync(a => a.Status == approvedStatus),
                RejectedRequests = await query.CountAsync(a => a.Status == rejectedStatus),
                PendingRequests = await query.CountAsync(a => a.Status == pendingStatus),
                DelayedRequests = await query.Where(a => a.Status == pendingStatus)
                                             .Include(a => a.ApplicationHistories)
                                             .CountAsync(a => a.ApplicationHistories.Any() && a.ApplicationHistories.OrderByDescending(h => h.ActionDate).First().ActionDate < delayedDate)
            };
        }

        // FIXED: This now shows CURRENT PENDING WORKLOAD per department, as requested.
        public async Task<ChartDataDTO<int>> GetRequestCountByDepartmentAsync(DateTime? startDate, DateTime? endDate)
        {
            var pendingStatus = AppStatues.قيد_التنفيذ.ToString();
            var query = GetFilteredApplications(startDate, endDate)
                .Where(a => a.Status == pendingStatus && a.Steps.Department != null);

            var result = await query
                .GroupBy(a => a.Steps.Department.DepartmentName)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Label, x => x.Count);

            var allDepartments = await _unitOfWork.Department.GetAllAsync();
            var dto = new ChartDataDTO<int>();
            foreach (var dept in allDepartments)
            {
                dto.Labels.Add(dept.DepartmentName);
                dto.Data.Add(result.ContainsKey(dept.DepartmentName) ? result[dept.DepartmentName] : 0);
            }
            return dto;
        }
        public async Task<ChartDataDTO<int>> GetCreatedRequestsOverTimeAsync(DateTime? startDate, DateTime? endDate)
        {
            // Set default date range if none provided, for example, the last 12 months.
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddYears(-1).AddDays(1);
            }

            var grouping = GetTimeGrouping(startDate, endDate);
            var query = GetFilteredApplications(startDate, endDate);
            var dto = new ChartDataDTO<int>();

            switch (grouping)
            {
                case TimeGrouping.Day:
                    var dailyData = await query
                        .GroupBy(a => a.CreatedDate.Date)
                        .Select(g => new { Label = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(x => x.Label, x => x.Count);

                    for (var dt = startDate.Value.Date; dt <= endDate.Value.Date; dt = dt.AddDays(1))
                    {
                        dto.Labels.Add(dt.ToString("yyyy-MM-dd"));
                        dto.Data.Add(dailyData.ContainsKey(dt) ? dailyData[dt] : 0);
                    }
                    break;

                case TimeGrouping.Year:
                    var yearlyData = await query
                        .GroupBy(a => a.CreatedDate.Year)
                        .Select(g => new { Label = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(x => x.Label, x => x.Count);

                    for (var year = startDate.Value.Year; year <= endDate.Value.Year; year++)
                    {
                        dto.Labels.Add(year.ToString());
                        dto.Data.Add(yearlyData.ContainsKey(year) ? yearlyData[year] : 0);
                    }
                    break;

                case TimeGrouping.Month:
                default:
                    var monthlyData = await query
                        .GroupBy(a => new { a.CreatedDate.Year, a.CreatedDate.Month })
                        .Select(g => new { Label = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Count() })
                        .ToDictionaryAsync(x => x.Label, x => x.Count);

                    var dateIterator = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);
                    while (dateIterator <= endDate.Value)
                    {
                        dto.Labels.Add($"{_arabicMonthNames[dateIterator.Month - 1]} {dateIterator.Year}");
                        dto.Data.Add(monthlyData.ContainsKey(dateIterator) ? monthlyData[dateIterator] : 0);
                        dateIterator = dateIterator.AddMonths(1);
                    }
                    break;
            }
            return dto;
        }

        // FIXED: This logic is now consistent with department-specific stats.
        public async Task<ChartDataDTO<int>> GetDepartmentStatusCountsAsync(string status, DateTime? startDate, DateTime? endDate)
        {
            var allDepartments = await _unitOfWork.Department.GetAllAsync();
            var dto = new ChartDataDTO<int>
            {
                Labels = allDepartments.Select(d => d.DepartmentName).ToList(),
                Data = new List<int>(new int[allDepartments.Count()])
            };

            // This loop calls the master GetDepartmentStatsAsync for each department
            // and then extracts the correct value based on the requested status.
            for (int i = 0; i < allDepartments.Count(); i++)
            {
                var deptId = allDepartments.ElementAt(i).DepartmentID;
                // Get the new, detailed stats for the current department in the loop.
                var deptStats = await GetDepartmentStatsAsync(deptId, startDate, endDate);

                // This switch statement now correctly maps the status to the new DTO structure.
                int count = status.ToLower() switch
                {
                    // "Approved" for a department is the sum of apps it created that were approved
                    // PLUS apps it received from others and approved itself.
                    "مقبول" => deptStats.CreatedByDepartment.AcceptedByOthers + deptStats.ReceivedFromOthers.AcceptedByDepartment,

                    // "Rejected" is the sum of apps it created that were rejected
                    // PLUS apps it received from others and rejected itself.
                    "مرفوض" => deptStats.CreatedByDepartment.RejectedByOthers + deptStats.ReceivedFromOthers.RejectedByDepartment,

                    // "Pending" on a global chart should show who has work IN THEIR INBOX right now.
                    // This is perfectly represented by the "InProgress" count of items received from others.
                    "قيد التنفيذ" => deptStats.ReceivedFromOthers.InProgress,

                    // "Delayed" is the subset of items pending in THEIR INBOX.
                    "متأخره" => deptStats.ReceivedFromOthers.Delayed,

                    // Default case for any unknown status.
                    _ => 0
                };

                dto.Data[i] = count;
            }

            return dto;
        }

        // FIXED: This now correctly calculates processing time for ALL departments, including the final one.
        public async Task<ChartDataDTO<double>> GetAverageProcessingTimeByDepartmentAsync(DateTime? startDate, DateTime? endDate)
        {
            var historiesQuery = _unitOfWork.ApplicationHistory.GetQueryable(null);
              

            if (startDate.HasValue)
            {
                historiesQuery = historiesQuery.Where(h => h.Application.CreatedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                historiesQuery = historiesQuery.Where(h => h.Application.CreatedDate < endDate.Value.AddDays(1));
            }
          

            var allHistories = await historiesQuery.Include(h => h.Application).ThenInclude(a=>a.CreatedByUser)
               .Include(h => h.Department)
                .OrderBy(h => h.ApplicationID).ThenBy(h => h.ActionDate)
                .ToListAsync();

            var durations = new List<(string DeptName, double Days)>();
            var historiesByApp = allHistories.GroupBy(h => h.ApplicationID);

            foreach (var appGroup in historiesByApp)
            {
                // Start with the application's creation date as the first "In-Date".
                DateTime inDate = appGroup.First().Application.CreatedDate;
                Department previousDepartment = appGroup.First().Application.CreatedByUser.Department;

                foreach (var historyItem in appGroup)
                {
                    // The date this action was taken is the "Out-Date" for the previous step.
                    DateTime outDate = historyItem.ActionDate;

                    // The duration is the time spent at the PREVIOUS department.
                    var duration = (outDate - inDate).TotalDays;

                    // Attribute the duration to the department that held the request before this action.
                    if (previousDepartment != null)
                    {
                        // We ensure duration is not negative. If it is, it's a data issue, and we treat it as 0 time.
                        durations.Add((previousDepartment.DepartmentName, Math.Max(0, duration)));
                    }

                    // For the next iteration, the current Out-Date becomes the new In-Date,
                    // and the current department becomes the new previous department.
                    inDate = outDate;
                    previousDepartment = historyItem.Department;
                }

                // Handle the time spent on requests that are still PENDING.
                var lastHistoryItem = appGroup.Last();
                var application = await _unitOfWork.Application
                    .GetQueryable(a => a.ApplicationID == lastHistoryItem.ApplicationID)
                    .Include(a => a.Steps.Department)
                    .FirstOrDefaultAsync();

                if (application != null && application.Status == AppStatues.قيد_التنفيذ.ToString())
                {
                    // The time spent so far at the current pending step.
                    var timeSinceLastAction = (DateTime.UtcNow - lastHistoryItem.ActionDate).TotalDays;

                    if (application.Steps?.Department != null)
                    {
                        durations.Add((application.Steps.Department.DepartmentName, Math.Max(0, timeSinceLastAction)));
                    }
                }
            }

            var result = durations
                .GroupBy(d => d.DeptName)
                .Select(g => new
                {
                    Label = g.Key,
                    AvgDays = g.Average(x => x.Days)
                })
                .ToDictionary(x => x.Label, x => Math.Round(x.AvgDays, 2));

            var allDepartments = await _unitOfWork.Department.GetAllAsync();
            var dto = new ChartDataDTO<double>();
            foreach (var dept in allDepartments)
            {
                dto.Labels.Add(dept.DepartmentName);
                dto.Data.Add(result.ContainsKey(dept.DepartmentName) ? result[dept.DepartmentName] : 0.0);
            }
            return dto;
        }
        #endregion

        #region Department-Specific Reports (Fully Corrected)

        // FIXED: This is the master source of truth for department stats, ensuring consistency.
        // In ReportService.cs

        // In ReportService.cs

        // In ReportService.cs

        // In ReportService.cs

        public async Task<DepartmentStatsDTO> GetDepartmentStatsAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            // --- 1. DEFINE STATUSES & GET THE DATA ---
            var pendingStatus = AppStatues.قيد_التنفيذ.ToString();
            var approvedStatus = AppStatues.مقبول.ToString();
            var rejectedStatus = AppStatues.مرفوض.ToString();
            var delayedDate = DateTime.UtcNow.AddDays(-DelayedThresholdDays);

            // Get a base query with date filters applied
            var baseQuery = GetFilteredApplications(startDate, endDate).Include(a => a.ApplicationType)
                .Include(a => a.Steps)
                .Include(a => a.ApplicationHistories)
                .Include(a => a.CreatedByUser);

            // Get applications where the TYPE was created by this department.
            // This is the "Created" dataset.
            var createdApplications = await baseQuery
                .Where(a => a.ApplicationType.CreatedByDeptId == departmentId)
                .ToListAsync();

            // ===================================================================
            // START: CORRECTED LOGIC FOR "RECEIVED" DATASET
            // ===================================================================
            // The "Received" dataset must include both historically processed items AND items currently in the inbox.
            // This is the key fix.
            var receivedApplications = await baseQuery
                .Where(a =>
                    // Rule 1: The application type was NOT created by this department.
                    a.ApplicationType.CreatedByDeptId != departmentId &&
                    (
                        // Rule 2a: The application has a history record of this department taking an action (approve/reject).
                        a.ApplicationHistories.Any(h => h.ActionByDeptId == departmentId && (h.Action == "موافقة" || h.Action == "رفض")) ||
                        // OR Rule 2b: The application is CURRENTLY pending in this department's inbox.
                        (a.Status == pendingStatus && a.Steps.DepartmentID == departmentId)

                    )
                )
                .ToListAsync();
            // ===================================================================
            // END: CORRECTED LOGIC FOR "RECEIVED" DATASET
            // ===================================================================

            // --- 2. CALCULATE "CREATED BY DEPARTMENT" STATS ---
            // This logic remains correct.
            var createdByDeptStats = new CreatedByDepartmentStatsDTO
            {
                Total = createdApplications.Count,
                InProgress = createdApplications.Count(a => a.Status == pendingStatus),
                Delayed = createdApplications
                    .Where(a => a.Status == pendingStatus)
                    .Count(a => a.ApplicationHistories.Any() && a.ApplicationHistories.Max(h => h.ActionDate) < delayedDate),
                AcceptedByOthers = createdApplications.Count(a => a.Status == approvedStatus),
                RejectedByOthers = createdApplications.Count(a => a.Status == rejectedStatus)
            };

            // --- 3. CALCULATE "RECEIVED FROM OTHERS" STATS ---
            // Now this will be calculated from the correct, comprehensive "receivedApplications" list.
            var receivedFromOthersStats = new ReceivedFromOthersStatsDTO
            {
                // "InProgress" now correctly counts the pending items from the `receivedApplications` list.
                InProgress = receivedApplications
                    .Count(a => a.Status == pendingStatus && a.Steps.DepartmentID == departmentId),

                Delayed = receivedApplications
                    .Where(a => a.Status == pendingStatus && a.Steps.DepartmentID == departmentId)
                    .Count(a => a.ApplicationHistories.Any() && a.ApplicationHistories.Max(h => h.ActionDate) < delayedDate),

                // These counts are based on the final status of the received applications.
                AcceptedByDepartment = receivedApplications.Count(a => a.ApplicationHistories.Any(h=>h.ActionByDeptId==departmentId &&h.Action== "موافقة")) ,
                RejectedByDepartment = receivedApplications.Count(a => a.ApplicationHistories.Any(h => h.ActionByDeptId == departmentId && h.Action == "رفض"))
            };
            var totalr= receivedFromOthersStats.AcceptedByDepartment + receivedFromOthersStats.RejectedByDepartment +
                receivedFromOthersStats.InProgress ;
            var totalc = createdByDeptStats.AcceptedByOthers + createdByDeptStats.RejectedByOthers +
                createdByDeptStats.InProgress ;

            // --- 4. ASSEMBLE THE FINAL DTO ---
            return new DepartmentStatsDTO
            {
                // The total is now implicitly correct because the two lists are mutually exclusive.
                TotalRequests = totalc+totalr,
                CreatedByDepartment = createdByDeptStats,
                ReceivedFromOthers = receivedFromOthersStats
            };
        }
        public async Task<DepartmentStatusCountDTO> GetDepartmentRequestStatusCountsAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            // 1. Call the new, powerful GetDepartmentStatsAsync to get the detailed, structured data.
            var stats = await GetDepartmentStatsAsync(departmentId, startDate, endDate);

            // 2. Aggregate the detailed stats into the simple, flat DTO required by this endpoint.
            return new DepartmentStatusCountDTO
            {
                // "Approved" is the sum of apps the dept created that were approved PLUS apps they received and approved themselves.
                Approved = stats.CreatedByDepartment.AcceptedByOthers + stats.ReceivedFromOthers.AcceptedByDepartment,

                // "Rejected" is the sum of apps the dept created that were rejected PLUS apps they received and rejected themselves.
                Rejected = stats.CreatedByDepartment.RejectedByOthers + stats.ReceivedFromOthers.RejectedByDepartment,

                // "Pending" for a department's summary should ONLY reflect what is currently in THEIR inbox.
                Pending = stats.ReceivedFromOthers.InProgress,

                // "Delayed" is the subset of items that are pending in THEIR inbox.
                Delayed = stats.ReceivedFromOthers.Delayed
            };
        }

        // FIXED: This logic is confirmed correct by the tester's feedback.
        public async Task<ChartDataDTO<int>> GetRequestCountByRequestTypeAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            var query = GetCurrentDepartmentInbox(departmentId, startDate, endDate);
            var result = await query
                .Include(a => a.ApplicationType)
                .GroupBy(a => a.ApplicationType.ApplicationTypeName)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Label, x => x.Count);

            var relevantAppTypes = await _unitOfWork.ApplicationType
                .GetQueryable(at => at.Steps.Any(s => s.DepartmentID == departmentId))
                .ToListAsync();

            var dto = new ChartDataDTO<int>();
            foreach (var appType in relevantAppTypes)
            {
                dto.Labels.Add(appType.ApplicationTypeName);
                dto.Data.Add(result.ContainsKey(appType.ApplicationTypeName) ? result[appType.ApplicationTypeName] : 0);
            }
            return dto;
        }

        // FIXED: This now mirrors the logic of the global average time function but for specific request types.
        public async Task<ChartDataDTO<double>> GetAverageProcessingTimeByRequestTypeAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            var historiesQuery = _unitOfWork.ApplicationHistory.GetQueryable(h => h.ActionByDeptId == departmentId);
               
            if (startDate.HasValue) historiesQuery = historiesQuery.Where(h => h.Application.CreatedDate >= startDate.Value);
            if (endDate.HasValue) historiesQuery = historiesQuery.Where(h => h.Application.CreatedDate < endDate.Value.AddDays(1));

            var departmentHistories = await historiesQuery.Include(h => h.Application).ThenInclude(a => a.ApplicationType).OrderBy(h => h.ApplicationID).ThenBy(h => h.ActionDate).ToListAsync();
            var applicationIds = departmentHistories.Select(h => h.ApplicationID).Distinct();
            var fullAppHistories = await _unitOfWork.ApplicationHistory.GetQueryable(h => applicationIds.Contains(h.ApplicationID))
                .OrderBy(h => h.ApplicationID).ThenBy(h => h.ActionDate).ToListAsync();

            var durations = new List<(string AppTypeName, double Days)>();

            foreach (var appGroup in fullAppHistories.GroupBy(h => h.ApplicationID))
            {
                var appType = departmentHistories.First(h => h.ApplicationID == appGroup.Key).Application.ApplicationType;
                var inDate = appGroup.First().Application.CreatedDate;

                foreach (var historyItem in appGroup.Where(h => h.Action != "إنشاء الطلب"))
                {
                    if (historyItem.ActionByDeptId == departmentId)
                    {
                        var duration = (historyItem.ActionDate - inDate).TotalDays;
                        durations.Add((appType.ApplicationTypeName, duration));
                    }
                    inDate = historyItem.ActionDate;
                }
            }

            var result = durations.GroupBy(d => d.AppTypeName).ToDictionary(g => g.Key, g => Math.Round(g.Average(x => x.Days), 2));
            var relevantAppTypes = await _unitOfWork.ApplicationType.GetQueryable(at => at.Steps.Any(s => s.DepartmentID == departmentId)).ToListAsync();

            var dto = new ChartDataDTO<double>();
            foreach (var appType in relevantAppTypes)
            {
                dto.Labels.Add(appType.ApplicationTypeName);
                dto.Data.Add(result.ContainsKey(appType.ApplicationTypeName) ? result[appType.ApplicationTypeName] : 0.0);
            }
            return dto;
        }

        // The dynamic time analysis logic remains the same, as it correctly identifies received vs processed.
        public async Task<DepartmentTimeAnalysisDTO> GetDepartmentTimeAnalysisAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddYears(-1).AddDays(1);
            }

            var grouping = GetTimeGrouping(startDate, endDate);

            // ===================================================================
            //  PART 1: GATHER ALL "PROCESSED" and "RECEIVED" DATA
            // ===================================================================

            // --- Step 1.1: Get all actions PROCESSED by the department in the date range. ---
            // This will be our source for `processedData`.
            var processedActions = await _unitOfWork.ApplicationHistory
                .GetQueryable(h =>h.Application.ApplicationType.CreatedByDeptId!=departmentId && h.ActionByDeptId == departmentId &&
                                   h.ActionDate >= startDate.Value.Date && h.ActionDate < endDate.Value.AddDays(1).Date
                                   )
                .Include(h => h.Application).ThenInclude(h=>h.ApplicationType   )
                .Select(h => new { h.ApplicationID, h.ActionDate, CreatedDate = h.Application.CreatedDate })
                .ToListAsync();

            var masterReceivedDates = new List<DateTime>();

            // --- Step 1.2: For each PROCESSED action, find its arrival date. ---
            if (processedActions.Any())
            {
                var processedAppIds = processedActions.Select(h => h.ApplicationID).Distinct();
                var fullHistoriesForProcessedApps = await _unitOfWork.ApplicationHistory
                    .GetQueryable(h => processedAppIds.Contains(h.ApplicationID))
                    .OrderBy(h => h.ApplicationID).ThenBy(h => h.ActionDate)
                    .Select(h => new { h.ApplicationID, h.ActionDate })
                    .ToListAsync();

                foreach (var action in processedActions)
                {
                    var previousAction = fullHistoriesForProcessedApps
                        .Where(h => h.ApplicationID == action.ApplicationID && h.ActionDate < action.ActionDate)
                        .OrderByDescending(h => h.ActionDate)
                        .FirstOrDefault();

                    masterReceivedDates.Add(previousAction?.ActionDate ?? action.CreatedDate);
                }
            }

            // --- Step 1.3: Get all applications CURRENTLY PENDING in the department's inbox. ---
            // This is the CRUCIAL NEW LOGIC to include items that have been received but not yet processed.
            var pendingStatus = AppStatues.قيد_التنفيذ.ToString();
            var pendingApplications = await _unitOfWork.Application
                .GetQueryable(a => a.Status == pendingStatus && a.Steps.DepartmentID == departmentId)
                .Include(a => a.ApplicationHistories)
                .ToListAsync();

            foreach (var app in pendingApplications)
            {
                // Find the arrival date, which is the date of the last action taken on the app.
                var arrivalDate = app.ApplicationHistories
                    .OrderByDescending(h => h.ActionDate)
                    .FirstOrDefault()?.ActionDate ?? app.CreatedDate;

                // IMPORTANT: Only add it to our list if the ARRIVAL date is within the selected date range.
                if (arrivalDate >= startDate.Value.Date && arrivalDate < endDate.Value.AddDays(1).Date)
                {
                    masterReceivedDates.Add(arrivalDate);
                }
            }

            // ===================================================================
            //  PART 2: GROUP AND BUILD THE DTO
            // ===================================================================
            var dto = new DepartmentTimeAnalysisDTO();
            switch (grouping)
            {
                case TimeGrouping.Day:
                    var processedDaily = processedActions.GroupBy(h => h.ActionDate.Date).ToDictionary(g => g.Key, g => g.Count());
                    var receivedDaily = masterReceivedDates.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count());
                    for (var dt = startDate.Value.Date; dt <= endDate.Value.Date; dt = dt.AddDays(1))
                    {
                        dto.Labels.Add(dt.ToString("yyyy-MM-dd"));
                        dto.ProcessedData.Add(processedDaily.ContainsKey(dt) ? processedDaily[dt] : 0);
                        dto.ReceivedData.Add(receivedDaily.ContainsKey(dt) ? receivedDaily[dt] : 0);
                    }
                    break;
                case TimeGrouping.Year:
                    var processedYearly = processedActions.GroupBy(h => h.ActionDate.Year).ToDictionary(g => g.Key, g => g.Count());
                    var receivedYearly = masterReceivedDates.GroupBy(d => d.Year).ToDictionary(g => g.Key, g => g.Count());
                    for (var year = startDate.Value.Year; year <= endDate.Value.Year; year++)
                    {
                        dto.Labels.Add(year.ToString());
                        dto.ProcessedData.Add(processedYearly.ContainsKey(year) ? processedYearly[year] : 0);
                        dto.ReceivedData.Add(receivedYearly.ContainsKey(year) ? receivedYearly[year] : 0);
                    }
                    break;
                case TimeGrouping.Month:
                default:
                    var processedMonthly = processedActions.GroupBy(h => new { h.ActionDate.Year, h.ActionDate.Month }).ToDictionary(g => new DateTime(g.Key.Year, g.Key.Month, 1), g => g.Count());
                    var receivedMonthly = masterReceivedDates.GroupBy(d => new { d.Year, d.Month }).ToDictionary(g => new DateTime(g.Key.Year, g.Key.Month, 1), g => g.Count());
                    var dateIterator = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);
                    while (dateIterator <= endDate.Value)
                    {
                        var monthKey = new DateTime(dateIterator.Year, dateIterator.Month, 1);
                        dto.Labels.Add($"{_arabicMonthNames[dateIterator.Month - 1]} {dateIterator.Year}");
                        dto.ProcessedData.Add(processedMonthly.ContainsKey(monthKey) ? processedMonthly[monthKey] : 0);
                        dto.ReceivedData.Add(receivedMonthly.ContainsKey(monthKey) ? receivedMonthly[monthKey] : 0);
                        dateIterator = dateIterator.AddMonths(1);
                    }
                    break;
            }
            return dto;
        }
        public async Task<ChartDataDTO<int>> GetRejectionReasonsAsync(int departmentId, DateTime? startDate, DateTime? endDate)
        {
            var rejectedStatus = AppStatues.مرفوض.ToString();
            var rejectionAction = "رفض";
            var rejectionNotes = await GetHistoricalDepartmentApplications(departmentId, startDate, endDate)
                .Where(a => a.Status == rejectedStatus)
                .SelectMany(a => a.ApplicationHistories)
                .Where(h => h.Action == rejectionAction && h.ActionByDeptId == departmentId && !string.IsNullOrEmpty(h.Notes))
                .Select(h => h.Notes)
                .ToListAsync();

            var result = rejectionNotes.GroupBy(note => note.Trim()).Select(g => new { Label = g.Key, Count = g.Count() }).ToList();
            return new ChartDataDTO<int> { Labels = result.Select(r => r.Label).ToList(), Data = result.Select(r => r.Count).ToList() };
        }

        #endregion
    }
}