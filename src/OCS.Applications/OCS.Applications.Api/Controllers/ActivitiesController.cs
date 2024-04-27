using Microsoft.AspNetCore.Mvc;
using OCS.Applications.Services.Activities;

namespace OCS.Applications.Api.Controllers;

/// <summary>
/// Управление видами активностей
/// </summary>
[Route("activities")]
public class ActivitiesController : ControllerBase
{
    /// <summary>
    /// Получение списка возможных типов активности
    /// </summary>
    /// <param name="activitiesService">Сервис работы с активностями</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <http code="200">Список типов активности</http>
    [HttpGet]
    public IActionResult GetActivities([FromServices] IActivitiesService activitiesService,
        CancellationToken cancellationToken)
    {
        var result = activitiesService.GetActivitiesAsync(cancellationToken);
        return result.Success is false
            ? new ObjectResult(new ProblemDetails { Title = "Internal server error", Detail = result.ErrorMessage })
                { StatusCode = 500 }
            : new OkObjectResult(result.Data);
    }
}