using EventTickets.Application.Exceptions;
using EventTickets.Middleware.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace EventTickets.Api.Middleware
{
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
                _logger.LogError(ex, "Unhandled exception occurred while processing request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var error = new ApiErrorResponse();

            switch (ex)
            {
                case NotFoundException nf:
                    response.StatusCode = StatusCodes.Status404NotFound;
                    error.StatusCode = response.StatusCode;
                    error.ErrorCode = "NOT_FOUND";
                    error.Message = nf.Message;
                    break;

                case ConflictException cf:
                    response.StatusCode = StatusCodes.Status409Conflict;
                    error.StatusCode = response.StatusCode;
                    error.ErrorCode = "CONFLICT";
                    error.Message = cf.Message;
                    break;

                case ValidationException ve:
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    error.StatusCode = response.StatusCode;
                    error.ErrorCode = "VALIDATION_ERROR";
                    error.Message = ve.Message;
                    error.Errors = ve.Errors; 
                    break;

                default:
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    error.StatusCode = response.StatusCode;
                    error.ErrorCode = "INTERNAL_SERVER_ERROR";
                    error.Message = "An unexpected error occurred.";
                    error.Details = ex.Message;
                    break;
            }

            error.TraceId = context.TraceIdentifier;


            var json = JsonSerializer.Serialize(error);
            return response.WriteAsync(json);
        }
    }
}
