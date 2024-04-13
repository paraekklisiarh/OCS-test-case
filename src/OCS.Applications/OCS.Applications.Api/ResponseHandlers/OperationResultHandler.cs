using Microsoft.AspNetCore.Mvc;
using OCS.Applications.Contracts;
using OCS.Applications.Services.Applications;

namespace OCS.Applications.Api.ResponseHandlers;

/// <summary>
/// Хелпер для обработки результата выполнения операции сервисом
/// </summary>
public static class OperationResultHandler
{
    /// <summary>
    /// Обрабатывает ошибку работы сервиса
    /// </summary>
    /// <param name="result">Результат выполнения операции</param>
    /// <returns>Корректный статус-код</returns>
    internal static IActionResult ReturnError(OperationResult<ApplicationDto> result)
    {
        if (result.Status is OperationResultType.Success)
            throw new ArgumentException("Success result can't be processed in this method", nameof(result));

        return result.Status switch
        {
            OperationResultType.Conflict => new ConflictResult(),

            OperationResultType.NotFound => new NotFoundResult(),

            OperationResultType.ValidationError => new ObjectResult(new ProblemDetails
                { Status = 400, Title = "Application validation error", Detail = result.ErrorMessage }),

            OperationResultType.Forbidden => new ObjectResult(new ProblemDetails
                { Status = 403, Title = "Operation forbidden", Detail = result.ErrorMessage }) { StatusCode = 403 },

            _ => new ObjectResult(new ProblemDetails
                { Title = "Internal server error", Detail = result.ErrorMessage }) { StatusCode = 500 }
        };
    }
}