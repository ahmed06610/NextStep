using AutoMapper;
using NextStep.Core.DTOs.Department;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;

namespace NextStep.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllAsync()
        {
            try
            {
                var departments = await _unitOfWork.Department.GetAllAsync();
                return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while fetching departments.", ex);
            }
        }

        public async Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto)
        {
            try
            {
                var department = _mapper.Map<Department>(dto);
                await _unitOfWork.Department.AddAsync(department);
                await _unitOfWork.CompleteAsync();
                return _mapper.Map<DepartmentDTO>(department);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while creating the department.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var department = await _unitOfWork.Department.GetByIdAsync(id);
                if (department == null)
                    return false;

                await _unitOfWork.Department.DeleteAsync(department);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while deleting the department.", ex);
            }
        }
    }
}
