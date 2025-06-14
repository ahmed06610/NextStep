using AutoMapper;
using NextStep.Core.Const;
using NextStep.Core.DTOs.Application;
using NextStep.Core.DTOs.Auth;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextStep.Core.Helper;

namespace NextStep.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ApplicationService(
            IUnitOfWork unitOfWork,
            IAuthService authService,
            IFileService fileService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<List<ApplicationStudent>> GetApplicationsForStuent(int StudentId)
        {
            try
            {
                var AppliationsForStudent = await _unitOfWork.Application.GetQueryable(ap => ap.StudentID == StudentId)
                    .Include(ap => ap.Steps)
                    .Include(ap => ap.ApplicationType).ToListAsync();
                var res = new List<ApplicationStudent>();

                foreach (var app in AppliationsForStudent)
                {
                    var steps = await _unitOfWork.Steps.GetQueryable(s => s.ApplicationTypeID == app.ApplicationTypeID).ToListAsync();
                    var step = steps.FirstOrDefault(s => s.StepsID == app.StepID);
                    var current = steps.IndexOf(step)+1;

                    var pro = current >= 0 && steps.Count > 0 ? (decimal)current / steps.Count : 0;

                    var ap = new ApplicationStudent
                    {
                        Id = app.ApplicationID,
                        Name = app.ApplicationType.ApplicationTypeName,
                        Progress = pro
                    };

                    res.Add(ap);
                }

                return res;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetApplicationsForStuent: {ex.Message}");
                throw;
            }
        }

        public async Task<Application> CreateApplicationAsync(CreateApplicationDTO dto, int employeeId)
        {
            try
            {
                var student = await _unitOfWork.Student.GetByNaidAsync(dto.StudentNaid);
               


                if (student == null)
                {
                    var registerDto = new RegisterStudentDTO
                    {
                        UserName = dto.StudentName,
                        Email = $"{dto.StudentNaid}@univ.edu",
                        Password = dto.StudentNaid,
                        Naid = dto.StudentNaid
                    };

                    var authResult = await _authService.RegisterStudentAsync(registerDto);
                    student = await _unitOfWork.Student.GetByAppUserIdAsync(authResult.UserId);
                }
                else
                {
                    if (student.User.UserName != dto.StudentName ||(student.User.PhoneNumber!=null  && student.User.PhoneNumber!=dto.StudentPhone))
                    {
                        throw new Exception("الطالب موجود بالفعل ولكن اسم الطالب أو رقم الهاتف غير متطابق مع البيانات المدخلة. يرجى التأكد من صحة البيانات المدخلة.");

                    }
                }

                var appsForStudent = await _unitOfWork.Application.GetQueryable(a => a.StudentID == student.StudentID 
               
                && a.Status != AppStatues.مرفوض.ToString()
                )
                    .FirstOrDefaultAsync();
                if (appsForStudent != null)
                {
                    throw new Exception("الطالب لديه طلبات جاريه الان او تم الموافقه عليها. لا يمكن إنشاء طلب جديد في نفس الوقت.");
                }

                var initialStep = await _unitOfWork.Steps.GetInitialStepByApplicationType(dto.ApplicationTypeID);
                if (initialStep == null)
                    throw new Exception("No workflow steps defined for this application type");

                var nextstep = await _unitOfWork.Steps.GetNextStepAsync(dto.ApplicationTypeID, initialStep.StepOrder);
                var filePath = await _fileService.SaveApplicationFileAsync(dto.Attachment, student.StudentID);

                var application = new Application
                {
                    ApplicationTypeID = dto.ApplicationTypeID,
                    Status = AppStatues.قيد_التنفيذ.ToString(),
                    CreatedBy = employeeId,
                    CreatedDate = TimeHelper.NowInEgypt,
                    StudentID = student.StudentID,
                    FileUpload = filePath,
                    Notes = dto.Notes,
                    StepID = nextstep.StepsID
                };

                await _unitOfWork.Application.AddAsync(application);
                await _unitOfWork.CompleteAsync();

                var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
                var history = new ApplicationHistory
                {
                    ApplicationID = application.ApplicationID,
                    ActionDate = TimeHelper.NowInEgypt,
                    ActionByDeptId = employee.DepartmentID,
                    Action = "إنشاء الطلب",
                    Notes = dto.Notes
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                return application;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in CreateApplicationAsync: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ApproveApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var application = await _unitOfWork.Application.GetByIdWithStepsAsync(dto.ApplicationID);
                if (application == null) return false;

                if (application.Steps.DepartmentID != departmentId)
                    return false;
                var apphis = await _unitOfWork.ApplicationHistory.GetQueryable(h => h.ApplicationID == dto.ApplicationID).OrderBy(h => h.ActionDate).LastOrDefaultAsync();
                if (application.Status != AppStatues.قيد_التنفيذ.ToString() || apphis.Action == "رفض")
                {
                    return false;
                }

                var nextStep = await _unitOfWork.Steps.GetNextStepAsync(
                    application.ApplicationTypeID,
                    application.Steps.StepOrder);

                if (nextStep != null)
                {
                    application.StepID = nextStep.StepsID;
                    application.Status = AppStatues.قيد_التنفيذ.ToString();
                }
                else
                {
                    application.Status = AppStatues.مقبول.ToString();
                }

                if (dto.Attachment != null)
                {
                    if (!string.IsNullOrEmpty(application.FileUpload))
                    {
                        _fileService.DeleteApplicationFile(application.FileUpload);
                    }

                    var filePath = await _fileService.SaveApplicationFileAsync(dto.Attachment, application.StudentID);
                    application.FileUpload = filePath;
                }

                _unitOfWork.Application.Update(application);

                var history = new ApplicationHistory
                {
                    ApplicationID = application.ApplicationID,
                    ActionDate = TimeHelper.NowInEgypt,
                    ActionByDeptId = departmentId,
                    Action = "موافقة",
                    Notes = dto.Notes
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();


                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                await transaction.RollbackAsync();

                Console.WriteLine($"Error in ApproveApplicationAsync: {ex.Message}");
                throw new Exception( ex.Message);
            }
        }

        public async Task<bool> RejectApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var application = await _unitOfWork.Application.GetByIdWithStepsAsync(dto.ApplicationID);
                if (application == null) return false;

                if (application.Steps.DepartmentID != departmentId)
                    return false;
                var apphis = await _unitOfWork.ApplicationHistory.GetQueryable(h => h.ApplicationID == dto.ApplicationID).OrderBy(h=>h.ActionDate).LastOrDefaultAsync();

                if (application.Status != AppStatues.قيد_التنفيذ.ToString() || apphis.Action == "رفض")
                {
                    return false;
                }

                if (dto.Attachment != null)
                {
                    if (!string.IsNullOrEmpty(application.FileUpload))
                    {
                        _fileService.DeleteApplicationFile(application.FileUpload);
                    }

                    var filePath = await _fileService.SaveApplicationFileAsync(dto.Attachment, application.StudentID);
                    application.FileUpload = filePath;
                }

                application.Status = AppStatues.مرفوض.ToString();
                _unitOfWork.Application.Update(application);


                var history = new ApplicationHistory
                {
                    ApplicationID = application.ApplicationID,
                    ActionDate = TimeHelper.NowInEgypt,
                    ActionByDeptId = departmentId,
                    Action = "رفض",
                    Notes = dto.Notes
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                await transaction.CommitAsync();


                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception
                Console.WriteLine($"Error in RejectApplicationAsync: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<InboxResponseDTO> GetInboxApplicationsAsync(
 int departmentId,
 bool isOrderCreatingDepartment,
 string search = null,
 int? requestType = null,
 string status = null,
 int page = 1,
 int limit = 10)
        {
            try
            {
                var query = _unitOfWork.Application.GetByCurrentDepartmentQueryable(departmentId);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(a =>
                        a.ApplicationID.ToString().Contains(search) ||
                        a.ApplicationType.ApplicationTypeName.Contains(search));
                }

                if (requestType.HasValue)
                {
                    query = query.Where(a => a.ApplicationTypeID == requestType.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Contains("طلب") || status.Contains("قيد"))
                        status = AppStatues.قيد_التنفيذ.ToString();
                    else if (status.Contains("مرفوض"))
                        status = AppStatues.مرفوض.ToString();
                    else if (status.Contains("مقبول"))
                        status = AppStatues.مقبول.ToString();

                    query = query.Where(a => a.Status == status);
                }

                var totalApplications = await query.CountAsync();

                var applications = await query
                    .OrderByDescending(a => a.CreatedDate)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var summary = new InboxSummaryDTO
                {
                    TotalApplications = totalApplications,
                    NewApplications = query.Count(a => a.Status == AppStatues.قيد_التنفيذ.ToString()),
                    AnsweredApplications = isOrderCreatingDepartment
                        ? query.Count(a => a.Status == AppStatues.مقبول.ToString() ||
                                                   a.Status == AppStatues.مرفوض.ToString())
                        : null
                };

                var applicationItems = applications.Select(a => new ApplicationListItemDTO
                {
                    ApplicationId = a.ApplicationID,
                    ApplicationType = a.ApplicationType.ApplicationTypeName,
                    SendingDepartment = a.ApplicationHistories
                        .OrderBy(h => h.ActionDate)
                        .FirstOrDefault()?.Department?.DepartmentName ?? "Unknown",
                    SentDate = a.CreatedDate,
                    Status = a.Status == AppStatues.قيد_التنفيذ.ToString()
                        ? AppStatues.طلب_جديد.ToString()
                        : a.Status,
                }).ToList();

                return new InboxResponseDTO
                {
                    Summary = summary,
                    Applications = applicationItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetInboxApplicationsAsync: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        public async Task<OutboxResponseDTO> GetOutboxApplicationsAsync(
       int departmentId,
       string search = null,
       int? requestType = null,
       string status = null,
       int page = 1,
       int limit = 10)
        {
            try
            {
                var query = _unitOfWork.Application.GetByCreatorOrActionDepartmentQueryable(departmentId);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(a =>
                        a.ApplicationID.ToString().Contains(search) ||
                        a.ApplicationType.ApplicationTypeName.Contains(search));
                }

                if (requestType.HasValue)
                {
                    query = query.Where(a => a.ApplicationTypeID == requestType.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Status == status);
                }

                var totalApplications = await query.CountAsync();
                var ApprovedApplications = await query
                    .Where(a => a.Status == AppStatues.مقبول.ToString())
                    .CountAsync();
                var RejectedApplications = await query
                    .Where(a => a.Status == AppStatues.مرفوض.ToString())
                    .CountAsync();

                var InProgressApplications = await query
                    .Where(a => a.Status == AppStatues.قيد_التنفيذ.ToString())
                    .CountAsync();
                var applications = await query
                    .OrderByDescending(a => a.CreatedDate)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var summary = new OutboxSummaryDTO
                {
                    TotalApplications = totalApplications,
                    ApprovedApplications = ApprovedApplications,
                    RejectedApplications = RejectedApplications,
                    InProgressApplications = InProgressApplications,
                };

                var applicationItems = applications.Select(a => new ApplicationListItemDTO
                {
                    ApplicationId = a.ApplicationID,
                    ApplicationType = a.ApplicationType.ApplicationTypeName,
                    SendingDepartment = a.Steps.Department.DepartmentName,
                    SentDate = a.CreatedDate,
                    Status = a.Status,
                }).ToList();

                return new OutboxResponseDTO
                {
                    Summary = summary,
                    Applications = applicationItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOutboxApplicationsAsync: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApplicationDetailsDTO> GetApplicationDetailsAsync(int applicationId, int empId = 0)
        {
            try
            {
                var application = await _unitOfWork.Application.GetQueryable(null)
                    .Include(a => a.ApplicationType)
                    .Include(a => a.CreatedByUser)
                        .ThenInclude(e => e.Department)
                    .Include(a => a.Steps)
                        .ThenInclude(s => s.Department)
                    .Include(a => a.ApplicationHistories)
                        .ThenInclude(h => h.Department)
                    .Include(a => a.Student)
                        .ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(a => a.ApplicationID == applicationId);

                if (application == null)
                    return null;

                var allSteps = await _unitOfWork.Steps.GetQueryable(s => s.ApplicationTypeID == application.ApplicationTypeID)
                    .OrderBy(s => s.StepOrder)
                    .Include(s => s.Department)
                    .ToListAsync();

                var stepDtos = allSteps.Select(s => new ApplicationStepDTO
                {
                    DepartmentName = s.Department.DepartmentName,
                    StepOrder = s.StepOrder,
                    IsCompleted = application.ApplicationHistories
                        .Any(h => h.Department.DepartmentID == s.DepartmentID &&
                                 (h.Action == "موافقة" || h.Action == "رفض" || h.Action == "إنشاء الطلب")),
                    IsCurrent = s.StepsID == application.StepID
                }).ToList();

                if (empId != 0)
                {

                    var deptofemp = (await _unitOfWork.Employee.GetQueryable(e => e.EmpID == empId)
                        .Include(e => e.Department)
                        .FirstOrDefaultAsync()).DepartmentID;

                    if (empId != 0 && deptofemp == application.CreatedByUser.DepartmentID && (application.Status == AppStatues.مقبول.ToString() || application.Status == AppStatues.مرفوض.ToString()))
                    {
                        application.IsDone = true;
                        _unitOfWork.Application.Update(application);
                    }
                }

                string applicationContext = "none"; // Default value
                if (empId != 0)
                {
                    var employee = await _unitOfWork.Employee.GetByIdAsync(empId);
                    if (employee != null)
                    {
                        // Check if the employee is the creator (outbox)
                        bool isCreator = application.CreatedBy == empId;

                        // Check if the employee's department is the current step department (inbox)
                        bool isCurrentDepartment = application.Steps?.DepartmentID == employee.DepartmentID;

                        // Determine the context
                        if (isCreator && isCurrentDepartment)
                        {
                            applicationContext = "Inbox";
                        }
                        else if (isCreator)
                        {
                            applicationContext = "outbox";
                        }
                        else if (isCurrentDepartment)
                        {
                            applicationContext = "inbox";
                        }
                        else if (isCreator &&
                                (application.Status == AppStatues.مقبول.ToString() ||
                                 application.Status == AppStatues.مرفوض.ToString()))
                        {
                            // Completed applications created by this employee
                            applicationContext = "outbox";
                        }
                    }
                }

                // ===================================================================
                // START: NEW LOGIC FOR CREATING HISTORY WITH IN-DATE AND OUT-DATE
                // ===================================================================

                var historyDtos = new List<HistoryItemDTO>();
                var sortedHistories = application.ApplicationHistories.OrderBy(h => h.ActionDate).ToList();

                // The InDate for the very first step is the application's creation date.
                DateTime nextInDate = application.CreatedDate;

                foreach (var historyItem in sortedHistories)
                {
                    historyDtos.Add(new HistoryItemDTO
                    {
                        // InDate is the OutDate of the previous step, or the application's create date for the first step.
                        InDate = nextInDate,

                        // OutDate is the date this department took action.
                        OutDate = historyItem.ActionDate,

                        // Standard properties
                        Action = historyItem.Action,
                        Department = historyItem.Department.DepartmentName,
                        Notes = historyItem.Notes
                    });

                    // The OutDate of this step becomes the InDate for the next step.
                    nextInDate = historyItem.ActionDate;
                }

                // ===================================================================
                // END: NEW LOGIC
                // ===================================================================


                return new ApplicationDetailsDTO
                {
                    ApplicationName = application.ApplicationType.ApplicationTypeName,
                    ApplicationId = application.ApplicationID,
                    CreatedDate = application.CreatedDate,
                    CreatedDepartment = application.CreatedByUser.Department.DepartmentName,
                    Notes = application.Notes,
                    FileUrl = application.FileUpload,
                    StudentName = application.Student.User.UserName,
                    StudentNId = application.Student.Naid,
                    Statue = application.Status,

                    // Assign the newly created list here
                    History = historyDtos,

                    Steps = stepDtos,
                    ApplicationContext = applicationContext,
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetApplicationDetailsAsync: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
