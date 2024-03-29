using Microsoft.AspNetCore.Mvc;
using OSC.Applications.Contracts;
using OSC.Applications.Contracts.Requests;
using OSC.Applications.Services.Applications;

namespace OSC.Applications.Api.Controllers;

/// <summary>
/// Управление заявками на участие
/// </summary>
[Route("applications")]
public class ApplicationsController(IApplicationsService applicationService) : ControllerBase
{
    /// <summary>
    /// Создание новой заявки на участие
    /// </summary>
    /// <param name="application">Заявка на участие</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Заявка создана</status>
    /// <status code="400">Заявка невалидна</status>
    /// <status code="409">Заявка уже существует</status>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationDto application,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return new BadRequestObjectResult(ModelState);

        var result = await applicationService.CreateAsync(application, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }

    /// <summary>
    /// Обновление заявки на участие
    /// </summary>
    /// <param name="applicationId">Идентификатор заявки</param>
    /// <param name="application">Обновленная заявка</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Заявка обновлена</status>
    /// <status code="400">Заявка невалидна</status>
    /// <status code="404">Заявка не найдена</status>
    [HttpPut]
    [Route("{applicationId:guid}")]
    public async Task<IActionResult> Update(Guid applicationId, [FromBody] UpdateApplicationDto application,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return new BadRequestObjectResult(ModelState);

        var result = await applicationService.UpdateAsync(applicationId, application,
            cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }

    /// <summary>
    /// Удаление черновика заявки
    /// </summary>
    /// <param name="applicationId">Идентификатор заявки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Заявка удалена</status>
    /// <status code="404">Заявка не найдена</status>
    [HttpDelete]
    [Route("{applicationId:guid}")]
    public async Task<IActionResult> Delete(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await applicationService.DeleteAsync(applicationId, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkResult();
    }

    /// <summary>
    /// Получение заявки по идентификатору
    /// </summary>
    /// <param name="applicationId">Идентификатор заявки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Заявка</status>
    /// <status code="404">Заявка не найдена</status>
    [HttpGet]
    [Route("{applicationId:guid}")]
    public async Task<IActionResult> Get(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await applicationService.GetAsync(applicationId, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }

    /// <summary>
    /// Подача заявки на рассмотрение
    /// </summary>
    /// <param name="applicationId">Идентификатор заявки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Заявка подана</status>
    /// <status code="404">Заявка не найдена</status>
    /// <status code="409">Заявка уже подана</status>
    /// <status code="422">Заявка невалидна</status>
    [HttpPost]
    [Route("{applicationId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await applicationService.SubmitAsync(applicationId, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkResult();
    }

    /// <summary>
    /// Получение списка заявок, поданных после указанной даты с параметром <see cref="submittedAfter" />;
    /// Получение списка заявок, не поданных до указанной даты с параметром <see cref="unsubmittedOlder" />
    /// </summary>
    /// <param name="submittedAfter">Дата, после которой необходимо получить заявки</param>
    /// <param name="unsubmittedOlder">Дата, до которой необходимо получить заявки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Список заявок</status>
    /// <status code="400">Передано два параметра либо ни одного</status>
    [HttpGet]
    public async Task<IActionResult> GetSubmittedAfter(
        [FromQuery] DateTimeOffset? unsubmittedOlder,
        [FromQuery] DateTimeOffset? submittedAfter,
        CancellationToken cancellationToken)
    {
        if (unsubmittedOlder.HasValue && submittedAfter.HasValue)
            return BadRequest("Both parameters cannot be specified in the same request.");

        OperationResult<ApplicationDto> result;
        if (unsubmittedOlder is not null)
            result = await applicationService.GetUnsubmittedOlderAsync(unsubmittedOlder.Value.ToUniversalTime(),
                cancellationToken);
        else if (submittedAfter is not null)
            result = await applicationService.GetSubmittedAfterAsync(submittedAfter.Value.ToUniversalTime(),
                cancellationToken);
        else return BadRequest("One of the parameters must be specified.");

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }

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
    [Route("/users/{authorId:guid}/currentapplication")]
    public async Task<IActionResult> GetUnsubmittedByAuthor(Guid authorId, CancellationToken cancellationToken)
    {
        var result = await applicationService.GetByAuthorAsync(authorId, cancellationToken);

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }

    /// <summary>
    /// Обрабатывает ошибку работы сервиса
    /// </summary>
    /// <param name="result">Результат выполнения операции</param>
    /// <returns>Корректный статус-код</returns>
    private static IActionResult ReturnError(OperationResult<ApplicationDto> result)
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