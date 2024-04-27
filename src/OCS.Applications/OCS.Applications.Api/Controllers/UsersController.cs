using Microsoft.AspNetCore.Mvc;
using OCS.Applications.Services.Applications;
using static OCS.Applications.Api.ResponseHandlers.OperationResultHandler;

namespace OCS.Applications.Api.Controllers;

/// <summary>
/// Управление пользователями
/// </summary>
[Route("users")]
public class UsersController(IApplicationsService applicationService) : ControllerBase
{
    /// <summary>
    /// Получение текущей не поданной заявки для указанного пользователя
    /// </summary>
    /// <param name="authorId">ID автора заявки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <remarks>ToDo: переопределяется корень роута</remarks>
    /// <http code="200">Заявка на участие</http>
    /// <http code="404">Заявка не найдена</http>
    /// <http code="404">Пользователь не найден</http>
    [HttpGet]
    [Route("{authorId:guid}/currentapplication")]
    public async Task<IActionResult> GetUnsubmittedByAuthor(Guid authorId, CancellationToken cancellationToken)
    {
        var result = await applicationService.GetUnsubmittedByAuthorAsync(authorId, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }
}