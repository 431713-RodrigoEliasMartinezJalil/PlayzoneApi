using System.Net;
using System.Text.Json;

namespace PlayzoneApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error no controlado en la aplicación.");
                await ManejarExcepcionAsync(httpContext, ex);
            }
        }
        private static Task ManejarExcepcionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var respuesta = new
            {
                statusCode = context.Response.StatusCode,
                mensaje = "Ocurrió un error interno en el servidor. Por favor intente más tarde.",
                detalle = exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
        }
    }
}
