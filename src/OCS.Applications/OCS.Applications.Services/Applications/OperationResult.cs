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
    public IEnumerable<T>? DataList { get; set; }
    
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

    public OperationResult(bool success, OperationResultType status, IEnumerable<T> dataList)
    {
        Success = success;
        DataList = dataList;
        Status = status;
    }
}
/// <summary>
/// Результат выполнения операции, ассоциированный с http-кодом
/// </summary>
public enum OperationResultType
{
    Error = 0,
    Success = 200,
    NotFound = 404,
    Conflict = 409,
    Forbidden = 403,
    ValidationError = 422,
}
