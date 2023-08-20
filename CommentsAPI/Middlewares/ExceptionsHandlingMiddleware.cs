using CommentsAPI.CustomExceptions;
using CommentsAPI.Models;
using CommentsAPI.Services.LoggerService;
using System.Net;

namespace CommentsAPI.Middlewares
{
    public class ExceptionsHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionsHandlingMiddleware(
            RequestDelegate next,
            ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DataProcessingException ex)
            {
                _logger.LogError($"Something went wrong: {ex.ErrorMessage}");
                await HandleDataProcessingExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(new ErrorDetails(
                context.Response.StatusCode, "Internal Server Error.").ToString());
            throw exception;
        }
        private async Task HandleDataProcessingExceptionAsync(HttpContext context, DataProcessingException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exception.StatusCode;
            await context.Response.WriteAsync(new ErrorDetails(
                context.Response.StatusCode, exception.ErrorMessage).ToString());
        }
    }
}
