using OCS.Applications.Domain.Repository;

namespace OCS.Applications.Infrastructure.Repositories;

public class ApplicationsRepositoryManager(IApplicationsRepository applicationsRepository, IUnitOfWork unitOfWork)
    : IRepositoryManager
{
    public IApplicationsRepository ApplicationsRepository { get; } = applicationsRepository;
    public IUnitOfWork UnitOfWork { get; } = unitOfWork;
}