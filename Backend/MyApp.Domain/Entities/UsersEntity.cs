using System.ComponentModel.DataAnnotations;

namespace MyApp.Domain.Entities
{
    public class UsersEntity
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? SecondName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string IdentificationNumber { get; set; } = string.Empty;
        public int IdentificationTypeId { get; set; }
        public int GenderId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int RoleId { get; set; } = 4; // Rol por defecto: Paciente
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;

        // Relaciones
        public IdentificationTypesEntity IdentificationType { get; set; } = null!;
        public GendersEntity Gender { get; set; } = null!;
        public RolesEntity Role { get; set; } = null!;
        public ICollection<UserVerificationsEntity> UserVerifications { get; set; } = null!;
        public ICollection<UserPasswordResetsEntity> UserPasswordResets { get; set; } = null!;
    }
}
