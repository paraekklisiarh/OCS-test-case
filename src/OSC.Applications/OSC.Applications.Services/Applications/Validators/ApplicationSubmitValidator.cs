using FluentValidation;
using OSC.Applications.Domain.Entitites;

namespace OSC.Applications.Services.Applications.Validators;

/// <summary>
/// Валидация заявки перед отправкой на рассмотрение
/// </summary>
public class ApplicationSubmitValidator : AbstractValidator<Application>
{
    public ApplicationSubmitValidator()
    {
        // можно отправить на рассмотрение только заявки у которых заполнены все обязательные поля
        RuleFor(a => a.AuthorId).NotEmpty();
        RuleFor(a => a.Activity).NotEmpty();
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Description).NotEmpty();
        RuleFor(a => a.Outline).NotEmpty();
    }
}