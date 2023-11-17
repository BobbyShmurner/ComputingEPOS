using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ComputingEPOS.Backend.Results;

[DefaultStatusCode(StatusCodes.Status403Forbidden)]
public class ForbiddenProblemResult : ProblemResult {
    public ForbiddenProblemResult(string? detail = null, string? instance = null, string? title = null, string? type = null) :
        base(detail, instance, StatusCodes.Status403Forbidden, title, type) {}
}