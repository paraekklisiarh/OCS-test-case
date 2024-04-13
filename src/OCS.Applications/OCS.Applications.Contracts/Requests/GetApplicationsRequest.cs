namespace OCS.Applications.Contracts.Requests;

/// <summary>
/// Запрос на получение списка заявок
/// </summary>
public class GetApplicationsRequest
{
    /// <summary>
    /// Дата, до которой заявки не были поданы
    /// </summary>
    public DateTimeOffset? UnsubmittedOlder { get; set; }
    
    /// <summary>
    /// Дата, после которой были поданы заявки
    /// </summary>
    public DateTimeOffset? SubmittedAfter { get; set; }
}