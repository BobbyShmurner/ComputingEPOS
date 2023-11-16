using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ComputingEPOS.Backend.Results;

[DefaultStatusCode(DefaultStatusCode)]
public class ProblemResult : ObjectResult {
    private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;

    public ProblemResult(string? detail = null, string? instance = null, int? statusCode = null, string? title = null, string? type = null) :
        base(new ProblemDetails {
                Detail = detail,
                Instance = instance,
                Status = statusCode ?? DefaultStatusCode,
                Title = title,
                Type = type,
            }
        )
    {
            StatusCode = statusCode ?? DefaultStatusCode;
    }
}