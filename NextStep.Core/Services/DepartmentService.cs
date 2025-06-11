using AutoMapper;
using NextStep.Core.DTOs.Department;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace NextStep.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;


        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
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
                // 1. Map the DTO to the Department entity
                var department = _mapper.Map<Department>(dto);
                await _unitOfWork.Department.AddAsync(department);

                // 2. Check if a role with the same name already exists
                var roleName = dto.DepartmentName.Trim();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    // 3. If it doesn't exist, create the new IdentityRole
                    var newRole = new IdentityRole(roleName);
                    var result = await _roleManager.CreateAsync(newRole);

                    // 4. If role creation fails, throw an exception to prevent saving the department
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to create role for department: {errors}");
                    }
                }

                // 5. Save both the new department and the new role (if created) in a single transaction.
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<DepartmentDTO>(department);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while creating the department: {ex.Message}", ex);
            }
        }
        // ===================================================================
        // END: ADJUSTED CreateAsync METHOD
        // ===================================================================

        // ===================================================================
        // START: ADJUSTED DeleteAsync METHOD (PROACTIVE IMPROVEMENT)
        // ===================================================================
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // 1. Find the department to be deleted
                var department = await _unitOfWork.Department.GetByIdAsync(id);
                if (department == null)
                    return false;

                // 2. Find the corresponding role
                var roleName = department.DepartmentName.Trim();
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    // 3. If the role exists, delete it
                    var result = await _roleManager.DeleteAsync(role);

                    // 4. If role deletion fails, throw an exception
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to delete role for department: {errors}");
                    }
                }

                // 5. Delete the department from the repository
                await _unitOfWork.Department.DeleteAsync(department);

                // 6. Save both deletions (department and role) in a single transaction.
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while deleting the department: {ex.Message}", ex);
            }
        }
    }
}
