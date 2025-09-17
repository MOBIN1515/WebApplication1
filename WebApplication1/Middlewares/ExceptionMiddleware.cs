using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره‌ای رخ داد!"); // ثبت لاگ
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var errorResponse = new
                {
                    Message = "یک خطای غیرمنتظره رخ داد.",
                    ErrorId = Guid.NewGuid()
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }


    }
}
