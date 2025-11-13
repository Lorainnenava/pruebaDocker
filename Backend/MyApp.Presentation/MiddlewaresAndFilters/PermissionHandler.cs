using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MyApp.Presentation.MiddlewaresAndFilters
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionHandler> _logger;

        public PermissionHandler(ILogger<PermissionHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                var endpoint = mvcContext.HttpContext.GetEndpoint();
                var requiredPermission = endpoint?.Metadata.GetMetadata<RequiredPermissionAttribute>()?.Permission;

                if (string.IsNullOrEmpty(requiredPermission))
                {
                    _logger.LogWarning("No se especificó ningún permiso requerido en el endpoint.");
                    return Task.CompletedTask;
                }

                _logger.LogInformation("Validando permiso requerido: {Permission}", requiredPermission);

                // Obtener rol del usuario (debe estar en el token como Claim "Role" o similar)
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "SuperAdmin")
                {
                    _logger.LogInformation("Acceso concedido automáticamente por ser SuperAdmin.");
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if (context.User.HasClaim("Permission", requiredPermission))
                {
                    _logger.LogInformation("Permiso concedido: el usuario tiene el permiso {Permission}", requiredPermission);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogWarning("Permiso denegado: el usuario NO tiene el permiso {Permission}", requiredPermission);
                }
            }
            else
            {
                _logger.LogWarning("El contexto de autorización no es AuthorizationFilterContext. No se puede validar el permiso.");
            }

            return Task.CompletedTask;
        }
    }
}
