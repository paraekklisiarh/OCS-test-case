using OCS.Applications.Contracts.Responses;
using OCS.Applications.Services.Applications;

namespace OCS.Applications.Services.Activities;

public interface IActivitiesService
{
    public OperationResult<ActivitiesResponse> GetActivitiesAsync(CancellationToken cancellationToken);
}