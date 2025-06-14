using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NextStep.Core.DTOs.Auth;
using NextStep.Core.Helper;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using NextStep.Core.Const;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NextStep.Core.Interfaces.Services;


namespace NextStep.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JWT> jwt,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuthResponseDTO> RegisterStudentAsync(RegisterStudentDTO model)
        {
            var authModel = new AuthResponseDTO();

            // Validate email exists
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                authModel.Message = "Email is already registered!";
                return authModel;
            }

            // Create user
            var user =  new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true // Set to false if email confirmation needed
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                authModel.Message = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return authModel;
            }

            // Add to Student role
            await _userManager.AddToRoleAsync(user, "طالب");

            // Create student record
            var student = new Student
            {
                UserId = user.Id,
                Naid = model.Naid,
                User = user
            };

            await _unitOfWork.Student.AddAsync(student);
            await _unitOfWork.CompleteAsync();

            // Generate token
            var jwtToken = await CreateJwtToken(user);

            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                Roles = new List<string> { "طالب" },
                UserId = user.Id,
                Email = user.Email,
                Name = user.UserName,
                LoggedId = student.StudentID,
                Role = "طالب"
            };
        }

        public async Task<AuthResponseDTO> RegisterEmployeeAsync(RegisterEmployeeDTO model)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var authModel = new AuthResponseDTO();

                // Validate email exists
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    authModel.Message = "Email is already registered!";
                    return authModel;
                }

                // Validate department exists
                var department = await _unitOfWork.Department.GetByIdAsync(model.DepartmentID);
                if (department == null)
                {
                    authModel.Message = "Invalid department specified";
                    return authModel;
                }

                // Create user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    authModel.Message = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return authModel;
                }

                // Determine role based on department
                string roleName = GetEmployeeRoleName(department.DepartmentName);
                await _userManager.AddToRoleAsync(user, roleName);

                // Create employee record
                var employee = new Employee
                {
                    UserId = user.Id,
                    DepartmentID = model.DepartmentID,
                    Department = department,
                    User = user
                };

                await _unitOfWork.Employee.AddAsync(employee);
                await _unitOfWork.CompleteAsync();

                // Commit transaction
                await transaction.CommitAsync();
                // Generate token
                var jwtToken = await CreateJwtToken(user);

                return new AuthResponseDTO
                {
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    ExpiresOn = jwtToken.ValidTo,
                    Roles = new List<string> { roleName },
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.UserName,
                    LoggedId = employee.EmpID,
                    Role = roleName,
                    Department = department.DepartmentName
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Registration failed. Transaction rolled back.", ex);

                throw ex;
            }
           
        }

        private string GetEmployeeRoleName(string departmentName)
        {
            var role = _unitOfWork.Department.GetAllAsync()
                .Result.FirstOrDefault(d => d.DepartmentName == departmentName)?.DepartmentName;

            return (role);
        }
        private static string GetRoleDisplayName(SystemRoles role)
        {
            var memberInfo = typeof(SystemRoles).GetMember(role.ToString()).FirstOrDefault();
            var displayAttribute = memberInfo?.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? role.ToString();
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
        {
            var authModel = new AuthResponseDTO();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "الايميل او كلمه المرور غير صحيحه";
                return authModel;
            }

            if (!user.EmailConfirmed)
            {
                authModel.Message = "الايميل لم يتم تأكيده";
                return authModel;
            }

            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            // Get additional user info based on role
            int loggedId = 0;
            string department = string.Empty;

            if (IsEmployeeRole(primaryRole))
            {
                var employee = await _unitOfWork.Employee.GetByAppUserIdAsync(user.Id);
                loggedId = employee?.EmpID ?? 0;
                department = GetDepartmentFromRole(primaryRole);
            }
            else if (primaryRole == "طالب")
            {
                var student = await _unitOfWork.Student.GetByAppUserIdAsync(user.Id);
                loggedId = student?.StudentID ?? 0;
            }

            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                Roles = roles.ToList(),
                UserId = user.Id,
                Email = user.Email,
                Name = user.UserName,
                LoggedId = loggedId,
                Role = primaryRole,
                Department = department
            };
        }

        private bool IsEmployeeRole(string role)
        {
            return role switch
            {
                "موظف مجلس الكليه" => true,
                "موظف لجنه الدرسات العليا" => true,
                "موظف حسابات علميه" => true,
                "موظف إدارة الدرسات العليا" => true,
                "موظف ذكاء اصطناعي" => true,
                "موظف نظم المعلومات" => true,
                "موظف علوم حاسب" => true,
                _ => false
            };
        }

        private string GetDepartmentFromRole(string role)
        {
            return role switch
            {
                "موظف مجلس الكليه" => "مجلس الكليه",
                "موظف لجنه الدرسات العليا" => "لجنة الدراسات العليا",
                "موظف حسابات علميه" => "حسابات علميه",
                "موظف إدارة الدرسات العليا" => "إدارة الدراسات العليا",
                "موظف ذكاء اصطناعي" => "ذكاء اصطناعي",
                "موظف نظم المعلومات" => "نظم المعلومات",
                "موظف علوم حاسب" => "علوم حاسب",
                _ => string.Empty
            };
        }

        public async Task<AuthResponseDTO> LoginStudentAsync(LoginStudentDTO model)
        {
            var authModel = new AuthResponseDTO();

            var user = await _userManager.FindByEmailAsync(model.NIdPassowrd+ "@univ.edu");
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.NIdPassowrd))
            {
                authModel.Message = "الايميل او كلمه المرور غير صحيحه";
                return authModel;
            }

            if (!user.EmailConfirmed)
            {
                authModel.Message = "الايميل لم يتم تأكيده";
                return authModel;
            }

            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            // Get additional user info based on role
            int loggedId = 0;
           
          
                var student = await _unitOfWork.Student.GetByAppUserIdAsync(user.Id);
                loggedId = student?.StudentID ?? 0;
            

            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                Roles = roles.ToList(),
                UserId = user.Id,
                Email = user.Email,
                Name = user.UserName,
                LoggedId = loggedId,
                Role = primaryRole,
            };
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var id = 0;
            Employee employee=null;
            var department = 0;

            if (roles.Contains("طالب"))
            {
                id = (await _unitOfWork.Student.GetByAppUserIdAsync(user.Id)).StudentID;

                 }
            else if (!roles.Contains("ادمن"))
            {
                id = (await _unitOfWork.Employee.GetByAppUserIdAsync(user.Id)).EmpID;
                employee = await _unitOfWork.Employee.FindTWithIncludes<Employee>(id, "EmpID", e => e.Department);
                department = employee.Department.DepartmentID;
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("DepartmentId",department.ToString()),
            new Claim("LoggedId", id.ToString()),// Id Of the LoggedIn User

           

        }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
