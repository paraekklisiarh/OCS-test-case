namespace OCS.Applications.Services.Applications;

/// <summary>
/// Представляет результат выполнения операции в сервисе
/// </summary>
/// <typeparam name="T">Возвращаемое значение</typeparam>
public sealed class OperationResult<T>
{
    public bool Success { get; set; }
    public OperationResultType Status { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public OperationResult(bool success, OperationResultType status)
    {
        Success = success;
        Status = status;
    }

    public OperationResult(bool success, OperationResultType status, T data)
    {
        Success = success;
        Data = data;
        Status = status;
    }
}
/// <summary>
/// Результат выполнения операции, ассоциированный с http-кодом
/// </summary>
public enum OperationResultType
{
    Error = 0,
    Success,
    NotFound,
    Conflict,
    Forbidden,
    ValidationError,
}
