using System.Text.Json;

namespace IEEE.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //await HandleExceptionAsync(context, ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n=== REAL ERROR: {ex.Message} ===");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string errorMessage = "An unexpected server error occurred.";

            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                errorMessage = exception.Message;
            }

            context.Response.StatusCode = statusCode;
            var responsePayload = JsonSerializer.Serialize(new { error = errorMessage });

            return context.Response.WriteAsync(responsePayload);
        }
    }
}
