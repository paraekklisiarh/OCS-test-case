using Microsoft.EntityFrameworkCore;
using OSC.Applications.Domain.Entitites;
using OSC.Applications.Domain.Repository;
using OSC.Applications.Infrastructure.Contexts;

namespace OSC.Applications.Infrastructure.Repositories;

internal sealed class ApplicationRepository : IApplicationsRepository
{
    private readonly AppDbContext _context;

    public ApplicationRepository(AppDbContext context)
    {
        _context = context;
    }


    public Task<Application?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        return _context.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
    }

    public Task<bool> AnyUnsubmittedByAuthorIdAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        return _context.Applications
            .AnyAsync(a => a.AuthorId == applicationId && a.Status == ApplicationStatus.Draft, cancellationToken);
    }

    public async Task<Application> AddAsync(Application application, CancellationToken cancellationToken)
    {
        await _context.AddAsync(application, cancellationToken);

        return application;
    }

    public void Update(Application application)
    {
        _context.Entry(application).State = EntityState.Modified;
    }

    public Task DeleteAsync(Application? application, CancellationToken cancellationToken)
    {
        _context.Applications.Remove(application);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Application?>> GetSubmittedAfterAsync(DateTimeOffset submittedAfter,
        CancellationToken cancellationToken)
    {
        return await _context.Applications.AsTracking().Where(a => a.SubmittedAt > submittedAfter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Application?>> GetUnsubmittedOlderAsync(DateTimeOffset olderThan,
        CancellationToken cancellationToken)
    {
        return await _context.Applications.AsTracking()
            .Where(a => a.Status == ApplicationStatus.Draft && a.CreatedAt < olderThan)
            .ToListAsync(cancellationToken);
    }

    public async Task<Application?> GetUnsubmittedByAuthorAsync(Guid authorId, CancellationToken cancellationToken)
    {
        return await _context.Applications.FirstOrDefaultAsync(
            x => x.AuthorId == authorId && x.Status == ApplicationStatus.Draft, cancellationToken);
    }
}