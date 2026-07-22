using System.Net;
using System.Text.Json;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.ServiceApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred. Please contact support.";

        // Handle specific SQL Exceptions for constraint violations
        if (exception is SqlException sqlEx)
        {
            statusCode = HttpStatusCode.BadRequest;
            
            // 547 = Foreign Key violation (usually trying to delete a record in use)
            if (sqlEx.Number == 547)
            {
                if (sqlEx.Message.Contains("DELETE"))
                {
                    message = "This record cannot be deleted because it is currently in use by other records in the system.";
                }
                else
                {
                    message = "This operation failed because it references a record that does not exist or is invalid.";
                }
            }
            // 2601 or 2627 = Unique Index/Constraint violation
            else if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            {
                message = "This record already exists. Please ensure the information is unique.";
            }
            else
            {
                message = $"A database error occurred. (Code: {sqlEx.Number})";
            }
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "You are not authorized to perform this action.";
        }
        else if (exception is ArgumentException || exception is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message; // Often contains safe business logic messages
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var envelope = new ApiEnvelope<object>
        {
            Success = false,
            Message = message,
            Data = null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(envelope, options));
    }
}
