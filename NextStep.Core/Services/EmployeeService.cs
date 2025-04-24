using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NextStep.Core.DTOs.Employee;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;

namespace NextStep.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employee.GetAllAsync();
            var employeeDTOs = new List<EmployeeDTO>();
            foreach (var emp in employees)
            {
                //get the role of the employee 
                var user = await _userManager.FindByIdAsync(emp.UserId);
                if (user == null)
                    continue;
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();
                var empDTO = _mapper.Map<EmployeeDTO>(emp);
                empDTO.Role = roleName;
                 employeeDTOs.Add(empDTO);
                
          }
            return employeeDTOs;
        }

        public async Task<bool> UpdateAsync(int id, UpdateEmployeeDTO dto)
        {
            var employee = await _unitOfWork.Employee.GetByIdAsync(id);
            if (employee == null)
                return false;

            var user = await _userManager.FindByIdAsync(employee.UserId);
            if (user == null)
                return false;

            user.UserName = dto.Name;
            user.Email = dto.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return false;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, dto.Password);
            }

            return true;
        }
    }
}
