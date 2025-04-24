using AutoMapper;
using NextStep.Core.DTOs.ApplicationType;
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
    public class ApplicationTypeService : IApplicationTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplicationTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationTypeDTO>> GetAllAsync(int departmentid=0)
        {
            var types = await _unitOfWork.ApplicationType
                .GetQueryable(at => at.CreatedByDeptId == departmentid||departmentid==0)
                .ToListAsync();

            if (types == null || !types.Any())
                return Enumerable.Empty<ApplicationTypeDTO>();

                return _mapper.Map<IEnumerable<ApplicationTypeDTO>>(types);
        }


        public async Task<ApplicationTypeDTO> GetByIdAsync(int id)
        {
            var type = await _unitOfWork.ApplicationType
                .GetQueryable(at => at.ApplicationTypeID == id)
                .Include(at => at.Requierments)
                    .ThenInclude(r => r.Requierment)
                .Include(at => at.Steps)
                .FirstOrDefaultAsync();

            if (type == null)
                throw new KeyNotFoundException("Application type not found");

            return new ApplicationTypeDTO
            {
                Id = type.ApplicationTypeID,
                Name = type.ApplicationTypeName,
                Requierments = type.Requierments.Select(r => new RequiermentDTO
                {
                    Id = r.Requierment.Id,
                    Name = r.Requierment.RequiermentName
                }).ToList(),
                Steps = type.Steps.Select(s => new StepDTO
                {
                    Id = s.StepsID,
                    DepartmentId = s.DepartmentID,
                    StepOrder = s.StepOrder
                }).ToList()
            };
        }


        public async Task<ApplicationTypeDTO> CreateAsync(CreateApplicationTypeDTO dto)
        {
            // Map the ApplicationType from the DTO
            var type = _mapper.Map<ApplicationType>(dto);

            // Add the ApplicationType to the database
            await _unitOfWork.ApplicationType.AddAsync(type);
            await _unitOfWork.CompleteAsync();

            // Handle adding requirements
            foreach (var reqDto in dto.createRequiermentDTOs)
            {
                // Create a new Requierment entity
                var requierment = new Requierments
                {
                    RequiermentName = reqDto.RequiermentName
                };

                // Add the Requierment to the database
                await _unitOfWork.Requierments.AddAsync(requierment);
                await _unitOfWork.CompleteAsync();

                // Create a new RequiermentsApplicationType entity to associate the requirement with the ApplicationType
                var requiermentApplicationType = new RequiermentsApplicationType
                {
                    ApplicationTypeId = type.ApplicationTypeID,
                    RequiermentId = requierment.Id
                };

                // Add the association to the database
                await _unitOfWork.RequiermentsApplicationType.AddAsync(requiermentApplicationType);
                await _unitOfWork.CompleteAsync();
            }

            // Handle adding steps
            foreach (var stepDto in dto.createStepsDTOs)
            {
                // Create a new Steps entity
                var step = new Steps
                {
                    ApplicationTypeID = type.ApplicationTypeID, // Assuming TransactionID is not provided in the DTO
                    DepartmentID = stepDto.DepartmentId,
                    StepOrder = stepDto.StepOrder
                };

                // Add the Step to the database
                await _unitOfWork.Steps.AddAsync(step);
                await _unitOfWork.CompleteAsync();
            }

            // Return the created ApplicationType as a DTO
            return _mapper.Map<ApplicationTypeDTO>(type);
        }


        public async Task<ApplicationTypeDTO> UpdateAsync(UpdateApplicationTypeDTO dto)
        {
            // Fetch the existing ApplicationType
            var existingType = await _unitOfWork.ApplicationType.GetByIdAsync(dto.Id);
            if (existingType == null)
                throw new KeyNotFoundException("Application type not found");

            // Update the basic properties of ApplicationType
            _mapper.Map(dto, existingType);
            _unitOfWork.ApplicationType.Update(existingType);
            await _unitOfWork.CompleteAsync();

            // Delete existing requirements and their associations
            var existingRequierments = await _unitOfWork.RequiermentsApplicationType.GetQueryable(rat => rat.ApplicationTypeId == dto.Id).ToListAsync();
            foreach (var reqAppType in existingRequierments)
            {
                await _unitOfWork.RequiermentsApplicationType.DeleteAsync(reqAppType);
                await _unitOfWork.Requierments.DeleteAsync(reqAppType.Requierment);
            }
            await _unitOfWork.CompleteAsync();

            // Add new requirements and their associations
            foreach (var reqDto in dto.Requierments)
            {
                var requierment = new Requierments
                {
                    RequiermentName = reqDto.RequiermentName
                };
                await _unitOfWork.Requierments.AddAsync(requierment);
                await _unitOfWork.CompleteAsync();

                var requiermentApplicationType = new RequiermentsApplicationType
                {
                    ApplicationTypeId = existingType.ApplicationTypeID,
                    RequiermentId = requierment.Id
                };
                await _unitOfWork.RequiermentsApplicationType.AddAsync(requiermentApplicationType);
            }
            await _unitOfWork.CompleteAsync();

            // Delete existing steps
            var existingSteps = await _unitOfWork.Steps.GetQueryable(s => s.ApplicationTypeID == dto.Id).ToListAsync();
            foreach (var step in existingSteps)
            {
                await _unitOfWork.Steps.DeleteAsync(step);
            }
            await _unitOfWork.CompleteAsync();

            // Add new steps
            foreach (var stepDto in dto.Steps)
            {
                var step = new Steps
                {
                    ApplicationTypeID = existingType.ApplicationTypeID,
                    DepartmentID = stepDto.DepartmentId,
                    StepOrder = stepDto.StepOrder
                };
                await _unitOfWork.Steps.AddAsync(step);
            }
            await _unitOfWork.CompleteAsync();

            // Return the updated ApplicationType as a DTO
            return _mapper.Map<ApplicationTypeDTO>(existingType);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var type = await _unitOfWork.ApplicationType.GetByIdAsync(id);
            if (type == null)
                return false;

           await _unitOfWork.ApplicationType.DeleteAsync(type);
            await _unitOfWork.CompleteAsync();
            return true;
        }

      
    }
}
