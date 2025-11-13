using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Application.Interfaces.UseCases.RefreshTokens;
using MyApp.Application.Interfaces.UseCases.UserPasswordResets;
using MyApp.Application.Interfaces.UseCases.Users;
using MyApp.Application.Interfaces.UseCases.UserSessions;
using MyApp.Application.Interfaces.UseCases.UserVerifications;
using MyApp.Application.Services;
using MyApp.Application.UseCases.Common;
using MyApp.Application.UseCases.RefreshTokens;
using MyApp.Application.UseCases.UserPasswordResets;
using MyApp.Application.UseCases.Users;
using MyApp.Application.UseCases.UserSessions;
using MyApp.Application.UseCases.UserVerifications;

namespace MyApp.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationUseCasesDependencies(this IServiceCollection services)
        {
            // Registro de casos de uso (implementaciones de las interfaces)
            services.AddScoped<IUserCreateUseCase, UserCreateUseCase>();
            services.AddScoped<IUserGetByIdUseCase, UserGetByIdUseCase>();
            services.AddScoped<IUserSetActiveStatusUseCase, UserSetActiveStatusUseCase>();
            services.AddScoped<IUserUpdateUseCase, UserUpdateUseCase>();
            services.AddScoped<IUserGetAllPaginatedUseCase, UserGetAllPaginatedUseCase>();
            services.AddScoped<IUserChangePasswordUseCase, UserChangePasswordUseCase>();

            services.AddScoped<IUserValidateUseCase, UserValidateUseCase>();
            services.AddScoped<IResendVerificationCodeUseCase, ResendVerificationCodeUseCase>();

            services.AddScoped<IUserSessionsCreateUseCase, UserSessionCreateUseCase>();
            services.AddScoped<IUserSessionRevokedUseCase, UserSessionRevokedUseCase>();

            services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();

            services.AddScoped<IUserPasswordResetRequestUseCase, UserPasswordResetRequestUseCase>();
            services.AddScoped<IUserPasswordResetConfirmUseCase, UserPasswordResetConfirmUseCase>();
            services.AddScoped<IUserPasswordResetValidateCodeUseCase, UserPasswordResetValidateCodeUseCase>();

            services.AddScoped(typeof(IGenericCreateUseCase<,>), typeof(GenericCreateUseCase<,>));
            services.AddScoped(typeof(IGenericGetAllPaginatedUseCase<,>), typeof(GenericGetAllPaginatedUseCase<,>));
            services.AddScoped(typeof(IGenericGetAllUseCase<,>), typeof(GenericGetAllUseCase<,>));
            services.AddScoped(typeof(IGenericGetOneUseCase<,>), typeof(GenericGetOneUseCase<,>));
            services.AddScoped(typeof(IGenericUpdateUseCase<,,>), typeof(GenericUpdateUseCase<,,>));
            services.AddScoped(typeof(IGenericDeleteUseCase<>), typeof(GenericDeleteUseCase<>));

            services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();

            return services;
        }
    }
}
