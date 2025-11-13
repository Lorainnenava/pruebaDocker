using System.ComponentModel.DataAnnotations;

namespace MyApp.Domain.Entities
{
    public class UserPasswordResetsEntity
    {
        [Key]
        public int UserPasswordResetId { get; set; }
        public int UserId { get; set; }
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordCodeExpiration { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;

        // Relaciones
        public UsersEntity User { get; set; } = null!;
    }
}
