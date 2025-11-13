namespace MyApp.Application.DTOs.UserPasswordResets
{
    public class UserPasswordResetValidateResetCodeRequest
    {
        public string ResetPasswordCode { get; set; } = string.Empty;
    }
}
