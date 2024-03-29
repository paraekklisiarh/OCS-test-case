using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OSC.Applications.Api.Filters;

public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        ProblemDetails problemDetails;
        switch (context.Exception)
        {
            case OperationCanceledException exception:
                problemDetails = new ProblemDetails
                {
                    Title = "Canceled",
                };
                logger.LogInformation("Operation {Trace} canceled.", exception.StackTrace);
                break;
            default:
                problemDetails = new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail =
                        $"Internal error on trace: {context.HttpContext.TraceIdentifier}",
                    Status = 500
                };
                logger.LogError(context.Exception, "Произошла неотловленная ошибка");
                break;
        }
        context.Result = new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        context.ExceptionHandled = true;
    }
}