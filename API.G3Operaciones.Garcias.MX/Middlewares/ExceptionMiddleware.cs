using API.G3Operaciones.Garcias.MX.Application.Common.Dto;
using API.G3Operaciones.Garcias.MX.Application.Common.Exceptions;
using System.Net;

namespace API.G3Operaciones.Garcias.MX.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(
                    ApiResponseDto.Fail(ex.Message)
                );
            }
            catch (InvalidOperationException ex)
            {
                // Capturamos errores de validación de reglas de negocio (como extensión sin cadena)
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(
                    ApiResponseDto.Fail(ex.Message)
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    ApiResponseDto.Fail(ex.Message)
                );
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    ApiResponseDto.Fail("Ocurrió un error interno")
                );
            }
        }
    }
}
