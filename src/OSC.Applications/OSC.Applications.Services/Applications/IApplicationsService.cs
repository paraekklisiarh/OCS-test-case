using OSC.Applications.Contracts;
using OSC.Applications.Contracts.Requests;

namespace OSC.Applications.Services.Applications;

/// <summary>
/// Сервис для работы с заявками
/// </summary>
public interface IApplicationsService
{
    public Task<OperationResult<ApplicationDto>> CreateAsync(CreateApplicationDto dto,
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

    public Task<OperationResult<ApplicationDto>> GetByAuthorAsync(Guid ownerId, CancellationToken cancellationToken);
}