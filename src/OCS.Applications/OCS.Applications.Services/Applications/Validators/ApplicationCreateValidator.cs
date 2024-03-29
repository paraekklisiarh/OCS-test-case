using FluentValidation;
using OCS.Applications.Contracts.Requests;

namespace OCS.Applications.Services.Applications.Validators;

/// <summary>
/// Валидация заявки при создании
/// </summary>
public class ApplicationCreateValidator : AbstractValidator<CreateApplicationDto>
{
    public ApplicationCreateValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty().WithMessage("нельзя создать заявку не указав идентификатор пользователя");
        
        RuleFor(x => x).Must(
            x => x.Activity is not null
                 || string.IsNullOrEmpty(x.Name) is false
                 || string.IsNullOrEmpty(x.Description) is false
                 || string.IsNullOrEmpty(x.Outline) is false
        ).WithMessage("нельзя создать заявку не указав хотя бы еще одно поле помимо идентификатора пользователя");
    }
}