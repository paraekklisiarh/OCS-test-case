using OCS.Applications.Contracts;
using OCS.Applications.Contracts.Requests;

namespace OCS.Applications.Services.Applications;

/// <summary>
/// Сервис для работы с заявками
/// </summary>
public interface IApplicationsService
{
    public Task<OperationResult<ApplicationDto>> CreateDraftAsync(CreateApplicationDto dto,
        CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> UpdateAsync(Guid applicationId, UpdateApplicationDto dto,
        CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> DeleteAsync(Guid applicationId, CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> GetAsync(Guid applicationId, CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> GetSubmittedAfterAsync(DateTimeOffset submittedAfter,
        CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> GetUnsubmittedOlderAsync(DateTimeOffset unsubmittedOlder,
        CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> SubmitAsync(Guid applicationId, CancellationToken cancellationToken);

    public Task<OperationResult<ApplicationDto>> GetUnsubmittedByAuthorAsync(Guid ownerId, CancellationToken cancellationToken);
}