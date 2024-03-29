using EnumFastToStringGenerated;
using OCS.Applications.Contracts.Responses;
using OCS.Applications.Services.Applications;

namespace OCS.Applications.Services.Activities;

public class ActivitiesService : IActivitiesService
{
    public OperationResult<ActivitiesResponse> GetActivitiesAsync(CancellationToken cancellationToken)
    {
        var activities = ActivityEnumExtensions.DisplayDescriptionsDictionary;

        var result = new OperationResult<ActivitiesResponse>(true, OperationResultType.Success)
        {
            DataList = activities
                .Select(pair => new ActivitiesResponse(activity: pair.Key.ToStringFast(), description: pair.Value))
                .ToList()
        };
        return result;
    }
}