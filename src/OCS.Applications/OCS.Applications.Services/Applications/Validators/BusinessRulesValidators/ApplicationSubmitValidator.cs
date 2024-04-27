using FluentValidation;
using OCS.Applications.Domain.Entitites;

namespace OCS.Applications.Services.Applications.Validators.BusinessRulesValidators;

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