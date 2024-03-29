namespace OSC.Applications.Contracts.Responses;

/// <summary>
/// Ответ хелсчека
/// </summary>
public sealed class HealthCheckResponse
{
    /// <summary>
    /// Статус здоровья API
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Проверяемые компоненты
    /// </summary>
    public IEnumerable<IndividualHealthCheckResponse> HealthChecks { get; set; } = null!;

    /// <summary>
    /// Длительность выполнения проверки здоровья API
    /// </summary>
    public TimeSpan HealthCheckDuration { get; set; }
}

/// <summary>
/// Результат хелсчека
/// </summary>
public sealed class IndividualHealthCheckResponse
{
    /// <summary>
    /// Статус здоровья компонента
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Название проверяемого компонента
    /// </summary>
    public required string Component { get; set; }

    /// <summary>
    /// Описание проверяемого компонента
    /// </summary>
    public string? Description { get; set; }
}