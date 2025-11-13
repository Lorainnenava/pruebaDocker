using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Seeders
{
    public static class DataSeeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Géneros
            modelBuilder.Entity<GendersEntity>().HasData(
                new GendersEntity { GenderId = 1, Name = "Masculino" },
                new GendersEntity { GenderId = 2, Name = "Femenino" },
                new GendersEntity { GenderId = 3, Name = "Homosexual" },
                new GendersEntity { GenderId = 4, Name = "Bisexual" },
                new GendersEntity { GenderId = 5, Name = "Otro" }
            );

            // Roles
            modelBuilder.Entity<RolesEntity>().HasData(
                new RolesEntity { RoleId = 1, Name = "SuperAdmin", CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
                new RolesEntity { RoleId = 2, Name = "Administrador", CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
                new RolesEntity { RoleId = 3, Name = "Doctor", CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
                new RolesEntity { RoleId = 4, Name = "Paciente", CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null }
            );

            // Tipos de identificación
            modelBuilder.Entity<IdentificationTypesEntity>().HasData(
                new IdentificationTypesEntity { IdentificationTypeId = 1, Name = "Cédula de Ciudadanía" },
                new IdentificationTypesEntity { IdentificationTypeId = 2, Name = "Tarjeta de Identidad" },
                new IdentificationTypesEntity { IdentificationTypeId = 3, Name = "Registro Civil" },
                new IdentificationTypesEntity { IdentificationTypeId = 4, Name = "Cédula de Extranjería" },
                new IdentificationTypesEntity { IdentificationTypeId = 5, Name = "Pasaporte" },
                new IdentificationTypesEntity { IdentificationTypeId = 6, Name = "Permiso por Protección Temporal" }
            );
        }
    }
}
