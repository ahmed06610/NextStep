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

        public async Task<IEnumerable<ApplicationTypeDTO>> GetAllAsync(int departmentid)
        {
            var types =await  _unitOfWork.ApplicationType.GetQueryable(at => at.CreatedByDeptId == departmentid).Include(at=>at.Requierments).ThenInclude(r=>r.Requierment).ToListAsync();
            if (types == null || !types.Any())
                return Enumerable.Empty<ApplicationTypeDTO>();


            return _mapper.Map<IEnumerable<ApplicationTypeDTO>>(types);
        }

        public async Task<ApplicationTypeDTO> GetByIdAsync(int id)
        {
            var type = await _unitOfWork.ApplicationType.GetByIdAsync(id);
            return _mapper.Map<ApplicationTypeDTO>(type);
        }

        public async Task<ApplicationTypeDTO> CreateAsync(CreateApplicationTypeDTO dto)
        {
            var type = _mapper.Map<ApplicationType>(dto);
            await _unitOfWork.ApplicationType.AddAsync(type);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ApplicationTypeDTO>(type);
        }

        public async Task<ApplicationTypeDTO> UpdateAsync(UpdateApplicationTypeDTO dto)
        {
            var existingType = await _unitOfWork.ApplicationType.GetByIdAsync(dto.Id);
            if (existingType == null)
                throw new KeyNotFoundException("Application type not found");

            _mapper.Map(dto, existingType);
            _unitOfWork.ApplicationType.Update(existingType);
            await _unitOfWork.CompleteAsync();
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
