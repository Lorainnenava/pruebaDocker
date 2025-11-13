using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Infrastructure.Repositories;
using MyApp.Infrastructure.Security;
using MyApp.Infrastructure.Services;

namespace MyApp.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IJwtHandler, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            return services;
        }
    }
}