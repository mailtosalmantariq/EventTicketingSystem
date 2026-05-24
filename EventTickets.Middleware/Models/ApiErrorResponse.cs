namespace EventTickets.Middleware.Models
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }

        public string ErrorCode { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }

        public string TraceId { get; set; } = string.Empty;
    }

}
