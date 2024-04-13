using FluentValidation;
using OCS.Applications.Contracts.Requests;

namespace OCS.Applications.Services.Applications.Validators.ExternalContractsValidators;

public class GetApplicationsValidator : AbstractValidator<GetApplicationsRequest>
{
    public GetApplicationsValidator()
    {
        RuleFor(x => x)
            .Must(x => !(x.UnsubmittedOlder.HasValue && x.SubmittedAfter.HasValue))
            .WithMessage("Both parameters cannot be specified in the same request.");
        
        RuleFor(x => x)
            .Must(x => x.UnsubmittedOlder.HasValue || x.SubmittedAfter.HasValue)
            .WithMessage("One of the parameters must be specified in the request.");
    }
}