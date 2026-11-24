using System.Net;

namespace Rent.UI.Services.Api
{
    // what every call returns
    public class ApiResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        // 400 and 409 are shown on the form; everything else is handled centrally
        public bool IsFormError =>
            StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Conflict;

        public static ApiResult<T> Fail(HttpStatusCode statusCode, string message) =>
            new() { StatusCode = statusCode, Success = false, Message = message };
    }
}
