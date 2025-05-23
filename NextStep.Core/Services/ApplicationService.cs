﻿using AutoMapper;
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
                    var current = steps.IndexOf(step);

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
                        UserName = dto.StudentNaid,
                        Email = $"{dto.StudentNaid}@univ.edu",
                        Password = dto.StudentNaid,
                        Naid = dto.StudentNaid
                    };

                    var authResult = await _authService.RegisterStudentAsync(registerDto);
                    student = await _unitOfWork.Student.GetByAppUserIdAsync(authResult.UserId);
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
                    CreatedDate = DateTime.Now,
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
                    ActionDate = DateTime.Now,
                    ActionByDeptId = employee.DepartmentID,
                    Action = "إنشاء الطلب",
                    Notes = "تم إنشاء الطلب"
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                return application;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in CreateApplicationAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ApproveApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId)
        {
            try
            {
                var application = await _unitOfWork.Application.GetByIdWithStepsAsync(dto.ApplicationID);
                if (application == null) return false;

                if (application.Steps.DepartmentID != departmentId)
                    return false;

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
                    ActionDate = DateTime.Now,
                    ActionByDeptId = departmentId,
                    Action = "موافقة",
                    Notes = dto.Notes
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ApproveApplicationAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RejectApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId)
        {
            try
            {
                var application = await _unitOfWork.Application.GetByIdWithStepsAsync(dto.ApplicationID);
                if (application == null) return false;

                if (application.Steps.DepartmentID != departmentId)
                    return false;

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
                    ActionDate = DateTime.Now,
                    ActionByDeptId = departmentId,
                    Action = "رفض",
                    Notes = dto.Notes
                };

                await _unitOfWork.ApplicationHistory.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in RejectApplicationAsync: {ex.Message}");
                throw;
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
                // Fetch the data from the repository
                var applications = await _unitOfWork.Application.GetByCurrentDepartmentAsync(departmentId);

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    applications = applications.Where(a =>
                        a.ApplicationID.ToString().Contains(search) ||
                        a.ApplicationType.ApplicationTypeName.Contains(search)).ToList();
                }

                // Apply request type filter
                if (requestType.HasValue)
                {
                    applications = applications.Where(a => a.ApplicationTypeID == requestType.Value).ToList();
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status))
                {
                    applications = applications.Where(a => a.Status == status).ToList();
                }

                // Pagination
                var totalApplications = applications.Count;
                applications = applications
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();

                // Prepare the summary
                var summary = new InboxSummaryDTO
                {
                    TotalApplications = totalApplications,
                    NewApplications = applications.Count(a => a.Status == AppStatues.قيد_التنفيذ.ToString()),
                    AnsweredApplications = isOrderCreatingDepartment
                        ? applications.Count(a => a.Status == AppStatues.مقبول.ToString() ||
                                                   a.Status == AppStatues.مرفوض.ToString())
                        : null
                };

                // Prepare the application items
                var applicationItems = applications.Select(a => new ApplicationListItemDTO
                {
                    ApplicationId = a.ApplicationID,
                    ApplicationType = a.ApplicationType.ApplicationTypeName,
                    SendingDepartment = a.ApplicationHistories
                        .OrderBy(h => h.ActionDate)
                        .FirstOrDefault()?.Department?.DepartmentName ?? "Unknown",
                    SentDate = a.CreatedDate,
                    Status = a.Status == AppStatues.قيد_التنفيذ.ToString() ? AppStatues.طلب_جديد.ToString() : a.Status,
                }).ToList();

                return new InboxResponseDTO
                {
                    Summary = summary,
                    Applications = applicationItems
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetInboxApplicationsAsync: {ex.Message}");
                throw;
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
                // Fetch the data from the repository
                var applications = await _unitOfWork.Application.GetByCreatorOrActionDepartmentAsync(departmentId);

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    applications = applications.Where(a =>
                        a.ApplicationID.ToString().Contains(search) ||
                        a.ApplicationType.ApplicationTypeName.Contains(search)).ToList();
                }

                // Apply request type filter
                if (requestType.HasValue)
                {
                    applications = applications.Where(a => a.ApplicationTypeID == requestType.Value).ToList();
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status))
                {
                    applications = applications.Where(a => a.Status == status).ToList();
                }

                // Pagination
                var totalApplications = applications.Count;
                applications = applications
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();

                // Prepare the summary
                var summary = new OutboxSummaryDTO
                {
                    TotalApplications = totalApplications,
                    ApprovedApplications = applications.Count(a => a.Status == AppStatues.مقبول.ToString()),
                    RejectedApplications = applications.Count(a => a.Status == AppStatues.مرفوض.ToString()),
                    InProgressApplications = applications.Count(a => a.Status == AppStatues.قيد_التنفيذ.ToString())
                };

                // Prepare the application items
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
                // Log the exception
                Console.WriteLine($"Error in GetOutboxApplicationsAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<ApplicationDetailsDTO> GetApplicationDetailsAsync(int applicationId)
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

                return new ApplicationDetailsDTO
                {
                    ApplicationName = application.ApplicationType.ApplicationTypeName,
                    ApplicationId = application.ApplicationID,
                    CreatedDate = application.CreatedDate,
                    CreatedDepartment = application.CreatedByUser.Department.DepartmentName,
                    Notes = application.Notes,
                    FileUrl = application.FileUpload,
                    Statue = application.Status,
                    History = application.ApplicationHistories
                        .OrderBy(h => h.ActionDate)
                        .Select(h => new HistoryItemDTO
                        {
                            ActionDate = h.ActionDate,
                            Action = h.Action,
                            Department = h.Department.DepartmentName,
                            Notes = h.Notes
                        }).ToList(),
                    Steps = stepDtos
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetApplicationDetailsAsync: {ex.Message}");
                throw;
            }
        }
    }
}
