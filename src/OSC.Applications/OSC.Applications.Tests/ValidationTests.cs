using OSC.Applications.Contracts.Requests;
using OSC.Applications.Domain.Entitites;
using OSC.Applications.Services.Applications.Validators;

namespace OSC.Applications.Tests;

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
}