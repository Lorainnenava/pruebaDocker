using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Domain.Entities
{
    [Index(nameof(CodeValidation), IsUnique = true)]
    public class UserVerificationsEntity
    {
        [Key]
        public int UserVerificationId { get; set; }
        public int UserId { get; set; }
        public string CodeValidation { get; set; } = string.Empty;
        public DateTime? CodeValidationExpiration { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;

        // Relaciones
        public UsersEntity User { get; set; } = null!;
    }
}
