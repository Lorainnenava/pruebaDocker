using MyApp.Shared.DTOs;
using System.Net;
using System.Text.Json;

namespace MyApp.Presentation.MiddlewaresAndFilters
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Si no está autenticado y se recibió un 401
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    StatusCode = 401,
                    ErrorMessage = "Acceso no autorizado. Debes iniciar sesión para obtener un token"
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
