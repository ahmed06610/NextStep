namespace NextStep.Core.DTOs.Auth
{
    // AuthResponseDTO.cs
    public class AuthResponseDTO
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Message { get; set; }
        public List<string> Roles { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int LoggedId { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
    }
}
