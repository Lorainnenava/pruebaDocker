using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyApp.Presentation.MiddlewaresAndFilters
{
    public class PublicFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica si el método o el controlador tienen [AllowAnonymous]
            var hasAllowAnonymous = context.ApiDescription.CustomAttributes()
            .Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));

            if (hasAllowAnonymous)
            {
                // Elimina los requisitos de seguridad para esta operación
                operation.Security?.Clear();
            }
            else
            {
                // Si no es pública, asegura que tenga el esquema de seguridad
                operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        ];
            }
        }
    }
}
