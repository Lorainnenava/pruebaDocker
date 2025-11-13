using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Seeders;

namespace MyApp.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }

        public DbSet<GendersEntity> Genders { get; set; }
        public DbSet<IdentificationTypesEntity> IdentificationTypes { get; set; }
        public DbSet<RefreshTokensEntity> RefreshTokens { get; set; }
        public DbSet<RolesEntity> Roles { get; set; }
        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<UserSessionsEntity> UserSessions { get; set; }
        public DbSet<UserVerificationsEntity> UserVerifications { get; set; }
        public DbSet<UserPasswordResetsEntity> UserPasswordResets { get; set; }
    }
}