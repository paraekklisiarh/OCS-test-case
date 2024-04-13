using Microsoft.AspNetCore.Mvc;
using OCS.Applications.Contracts;
using OCS.Applications.Contracts.Requests;
using OCS.Applications.Services.Activities;
using OCS.Applications.Services.Applications;
using static OCS.Applications.Api.ResponseHandlers.OperationResultHandler;

namespace OCS.Applications.Api.Controllers;

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

        var result = await applicationService.CreateDraftAsync(application, cancellationToken);

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
    /// Получение списка заявок, не поданных и старше указанной даты с параметром <see cref="unsubmittedOlder" />
    /// </summary>
    /// <param name="submittedAfter">Дата, после которой были поданы заявки</param>
    /// <param name="unsubmittedOlder">Дата, до которой заявки не были поданы</param>
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

        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.DataList);
    }

    /// <summary>
    /// Получение списка возможных типов активности
    /// </summary>
    /// <param name="activitiesService">Сервис работы с активностями</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <http code="200">Список типов активности</http>
    [HttpGet]
    [Route("/activities")]
    public IActionResult GetActivities([FromServices] IActivitiesService activitiesService,
        CancellationToken cancellationToken)
    {
        var result = activitiesService.GetActivitiesAsync(cancellationToken);
        return result.Success is false
            ? new ObjectResult(new ProblemDetails { Title = "Internal server error", Detail = result.ErrorMessage })
                { StatusCode = 500 }
            : new OkObjectResult(result.DataList);
    }
}