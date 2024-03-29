using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OCS.Applications.Contracts.Requests;
using OCS.Applications.Domain.Entitites;
using OCS.Applications.Services.Activities;
using OCS.Applications.Services.Applications;
using OCS.Applications.Services.Applications.Validators;

namespace OCS.Applications.Services;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует сервисы приложения в DI
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения</param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IApplicationsService, ApplicationsService>();
        services.AddTransient<IActivitiesService, ActivitiesService>();
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