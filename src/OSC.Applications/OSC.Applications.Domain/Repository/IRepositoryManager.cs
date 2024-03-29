namespace OSC.Applications.Domain.Repository;

public interface IRepositoryManager
{
    IApplicationsRepository ApplicationsRepository { get; }
    
    IUnitOfWork UnitOfWork { get; }
}