using OSC.Applications.Contracts.Responses;
using OSC.Applications.Services.Applications;

namespace OSC.Applications.Services.Activities;

public interface IActivitiesService
{
    public OperationResult<ActivitiesResponse> GetActivitiesAsync(CancellationToken cancellationToken);
}