using OCS.Applications.Contracts.Responses;
using OCS.Applications.Services.Applications;

namespace OCS.Applications.Services.Activities;

public interface IActivitiesService
{
    public OperationResult<List<ActivitiesResponse>> GetActivitiesAsync(CancellationToken cancellationToken);
}