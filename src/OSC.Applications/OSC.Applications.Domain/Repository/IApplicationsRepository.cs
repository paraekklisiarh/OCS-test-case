using OSC.Applications.Domain.Entitites;

namespace OSC.Applications.Domain.Repository;

public interface IApplicationsRepository
{
    Task<Application?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken);

    Task<bool> AnyUnsubmittedByAuthorIdAsync(Guid applicationId, CancellationToken cancellationToken);

    Task<Application> AddAsync(Application application, CancellationToken cancellationToken);

    void Update(Application application);

    Task DeleteAsync(Application application, CancellationToken cancellationToken);

    Task<IEnumerable<Application?>> GetSubmittedAfterAsync(DateTimeOffset submittedAfter,
        CancellationToken cancellationToken);

    Task<IEnumerable<Application?>> GetUnsubmittedOlderAsync(DateTimeOffset olderThan,
        CancellationToken cancellationToken);

    Task<Application?> GetUnsubmittedByAuthorAsync(Guid authorId, CancellationToken cancellationToken);
}