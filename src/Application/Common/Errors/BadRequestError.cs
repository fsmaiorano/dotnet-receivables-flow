using System.Net;

namespace Application.Common.Errors;

public class BadRequestError
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
    public required string Message { get; set; }
}