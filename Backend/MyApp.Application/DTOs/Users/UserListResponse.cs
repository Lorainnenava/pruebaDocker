namespace MyApp.Application.DTOs.Users
{
    public class UserListResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string IdentificationTypeName { get; set; } = string.Empty;
        public string IdentificationNumber { get; set; } = string.Empty;
        public string GenderName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsValidated { get; set; }
        public bool IsActived { get; set; }
    }
}
