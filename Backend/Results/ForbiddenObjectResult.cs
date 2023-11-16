using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ComputingEPOS.Backend.Results;

[DefaultStatusCode(DefaultStatusCode)]
public class ForbiddenObjectResult : ObjectResult {
    private const int DefaultStatusCode = StatusCodes.Status403Forbidden;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenObjectResult"/> class.
    /// </summary>
    /// <param name="value">The content to format into the entity body.</param>
    public ForbiddenObjectResult(object? value) : base(value) =>
        StatusCode = DefaultStatusCode;
}