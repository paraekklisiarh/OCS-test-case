using Microsoft.AspNetCore.Mvc;
using OCS.Applications.Contracts;
using OCS.Applications.Contracts.Requests;
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
    /// Получение списка заявок, поданных после указанной даты с параметром <see cref="GetApplicationsRequest.SubmittedAfter" />;
    /// Получение списка заявок, не поданных и старше указанной даты с параметром <see cref="GetApplicationsRequest.UnsubmittedOlder" />
    /// </summary>
    /// <param name="request">Запрос, содержащий параметры выборки <see cref="GetApplicationsRequest.SubmittedAfter"/>
    /// или <see cref="GetApplicationsRequest.UnsubmittedOlder"/></param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <status code="200">Список заявок</status>
    /// <status code="400">Переданы два параметра либо ни одного</status>
    [HttpGet]
    public async Task<IActionResult> GetApplications(
        [FromQuery] GetApplicationsRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return new BadRequestObjectResult(ModelState);
        
        OperationResult<List<ApplicationDto>> result = null!;
        if (request.UnsubmittedOlder is not null)
            result = await applicationService.GetUnsubmittedOlderAsync(request.UnsubmittedOlder.Value.ToUniversalTime(),
                cancellationToken);
        else if (request.SubmittedAfter is not null)
            result = await applicationService.GetSubmittedAfterAsync(request.SubmittedAfter.Value.ToUniversalTime(),
                cancellationToken);

        if (result is null)
            return new ObjectResult(new ProblemDetails
            { Title = "Internal server error", Detail = "Something went wrong in the server" }) { StatusCode = 500 };
        
        return result.Success is false ? ReturnError(result) : new OkObjectResult(result.Data);
    }
}