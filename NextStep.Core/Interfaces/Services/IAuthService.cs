using NextStep.Core.DTOs.Auth;
using NextStep.Core.Models;
using System.IdentityModel.Tokens.Jwt;

namespace NextStep.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO model);
        Task<AuthResponseDTO> LoginStudentAsync(LoginStudentDTO model);

        Task<AuthResponseDTO> RegisterStudentAsync(RegisterStudentDTO model);
        Task<AuthResponseDTO> RegisterEmployeeAsync(RegisterEmployeeDTO model);
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    }   
}
