using OSC.Applications.Domain.Repository;

namespace OSC.Applications.Infrastructure.Repositories;

public class ApplicationsRepositoryManager(IApplicationsRepository applicationsRepository, IUnitOfWork unitOfWork)
    : IRepositoryManager
{
    public IApplicationsRepository ApplicationsRepository { get; } = applicationsRepository;
    public IUnitOfWork UnitOfWork { get; } = unitOfWork;
}