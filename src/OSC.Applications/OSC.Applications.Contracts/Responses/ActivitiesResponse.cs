namespace OSC.Applications.Contracts.Responses;

/// <summary>
/// Список возможных типов активности
/// </summary>
public class ActivitiesResponse(string activity, string description)
{
    /// <summary>
    /// Тип активности
    /// </summary>
    public string Activity { get; set; } = activity;

    /// <summary>
    /// Описание типа активности
    /// </summary>
    public string Description { get; set; } = description;
}