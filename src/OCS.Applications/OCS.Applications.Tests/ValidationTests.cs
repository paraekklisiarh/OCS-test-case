using FluentValidation.TestHelper;
using OCS.Applications.Contracts.Requests;
using OCS.Applications.Domain.Entitites;
using OCS.Applications.Services.Applications.Validators.BusinessRulesValidators;
using OCS.Applications.Services.Applications.Validators.ExternalContractsValidators;

namespace OCS.Applications.Tests;

public class ValidationTests
{
    private readonly ApplicationCreateValidator _createValidator = new();

    private readonly ApplicationSubmitValidator _submitValidator = new();

    private readonly ApplicationUpdateValidator _updateValidator = new();

    // ToDo: to theory
    [Fact(DisplayName = "Нельзя создать заявку не указав хотя бы еще одно поле помимо идентификатора пользователя")]
    public void CanNotCreateWithoutSetFields()
    {
        var app = new CreateApplicationDto
            { AuthorId = Guid.NewGuid(), Activity = null, Description = null, Name = null, Outline = null };
        // Act
        var result = _createValidator.Validate(app);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Нельзя создать заявку не указав идентификатор пользователя")]
    public void CanNotCreateWithoutAuthorId()
    {
        var app = new CreateApplicationDto
        {
            AuthorId = Guid.Empty, Activity = Activity.Discussion, Description = "Correct", Name = "Valid",
            Outline = "Correct"
        };
        //
        // Act
        var result = _createValidator.Validate(app);

        // Assert
        Assert.False(result.IsValid);
    }

    // ToDo: to theory
    [Fact(DisplayName = "Можно отправить на рассмотрение только заявки у которых заполнены все обязательные поля")]
    public void CanNotSubmitWithoutSetFields()
    {
        var app = new Application(Guid.NewGuid(), null, "Valid Name", "Valid Description", "Valid Outline",
            DateTimeOffset.Now);

        var result = _submitValidator.Validate(app);

        Assert.False(result.IsValid);
    }

    // ToDo: to theory
    [Fact(DisplayName =
        "Нельзя отредактировать заявку так, чтобы  в ней не остались заполненными идентификатор пользователя + еще одно поле")]
    public void CanNotEditWithoutSetFields()
    {
        var app = new UpdateApplicationDto(null, null, null, null);

        var result = _updateValidator.Validate(app);

        Assert.False(result.IsValid);
    }
    
    [Fact(DisplayName = "запрос на получение поданных и не поданных заявок одновременно должен считаться не корректным")]
    public void BothParametersSpecified_ReturnsErrorMessage()
    {
        var request = new GetApplicationsRequest { UnsubmittedOlder = DateTimeOffset.Now, SubmittedAfter = DateTimeOffset.Now };
        var validator = new GetApplicationsValidator();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact(DisplayName = "При запросе списка заявок нужно указать один из параметров даты")]
    public void NeitherParameterSpecified_ReturnsErrorMessage()
    {
        var request = new GetApplicationsRequest{UnsubmittedOlder = null, SubmittedAfter = null};
        var validator = new GetApplicationsValidator();
        
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x);
    }
}