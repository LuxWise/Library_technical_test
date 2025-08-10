using System.Net;
using System.Text.Json;
using Library.DTO.Error;
using Library.Exeptions;

namespace Library.Middleware
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                await WriteErrorAsync(context, ex.Status, ex.Code, ex.Message, (ex as ValidationAppException)?.Errors);
            }
            catch
            {
                await WriteErrorAsync(context, (int)HttpStatusCode.InternalServerError, "internal_error",
                    "An unexpected error occurred.");
            }
        }

        private static Task WriteErrorAsync(HttpContext ctx, int status, string code, string? detail, Dictionary<string,string[]>? errors=null)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = status;

            var payload = new ErrorResponse()
            {
                TraceId = ctx.TraceIdentifier,
                Status = status,
                Code = code,
                Title = ToTitle(code),
                Detail = detail,
                Errors = errors
            };

            return ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, _json));
        }

        private static string ToTitle(string code) => code switch
        {
            "unauthorized"     => "Unauthorized",
            "forbidden"        => "Forbidden",
            "not_found"        => "Not Found",
            "conflict"         => "Conflict",
            "validation_error" => "Validation Error",
            _                  => "Error"
        };
    }
}

