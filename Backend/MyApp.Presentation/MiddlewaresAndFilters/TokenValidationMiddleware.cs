using System.IdentityModel.Tokens.Jwt;

namespace MyApp.Presentation.MiddlewaresAndFilters
{
    public class TokenValidationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            //  Permitir que las solicitudes OPTIONS pasen sin validación (CORS preflight)
            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            // Ignorar rutas públicas como Swagger y documentación
            var path = context.Request.Path.Value;
            if (path != null &&
                path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            // Verificar si la ruta actual permite acceso anónimo
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
            {
                await _next(context); // Continuar sin validar el token
                return;
            }

            // Obtener el token del encabezado Authorization
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token is missing");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Validar la expiración del token
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Token has expired");
                }
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            // Continuar con la solicitud si el token es válido
            await _next(context);
        }
    }
}
