using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using OSC.Applications.Contracts.Requests;
using OSC.Applications.Domain.Entitites;
using OSC.Applications.Services.Applications;
using OSC.Applications.Services.Applications.Validators;

namespace OSC.Applications.Services;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует сервисы приложения в DI
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения</param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IApplicationsService, ApplicationsService>();
    }

    /// <summary>
    /// Регистрирует валидаторы
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения</param>
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateApplicationDto>, ApplicationCreateValidator>();
        services.AddScoped<IValidator<UpdateApplicationDto>, ApplicationUpdateValidator>();
        services.AddScoped<IValidator<Application>, ApplicationSubmitValidator>();
    }
}