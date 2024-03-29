using EnumFastToStringGenerated;
using OSC.Applications.Contracts.Responses;
using OSC.Applications.Services.Applications;

namespace OSC.Applications.Services.Activities;

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