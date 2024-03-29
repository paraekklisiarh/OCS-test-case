using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using OCS.Applications.Api.Filters;
using OCS.Applications.Contracts.Responses;
using OCS.Applications.Infrastructure;
using OCS.Applications.Infrastructure.Contexts;
using OCS.Applications.Services;
using Serilog;

namespace OCS.Applications.Api;

/// <summary>
/// Основной класс
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа
    /// </summary>
    /// <param name="args">Параметры командной строки</param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;
        var env = builder.Environment;

// Infrastructure
        services.RegisterInfrastructure(configuration);

        services.AddControllers(options => { options.Filters.Add<ExceptionFilter>(); })
            .AddJsonOptions(options =>
            {
                // Добавляем конвертер для енама
                // По умолчанию енам преобразуется в цифровое значение
                // Этим конвертером задаем перевод в строковое занчение
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

//// Services
        services.AddFluentValidationAutoValidation();
        services.AddValidators();

        services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);

        services.RegisterServices();

        services.AddHealthChecks();

//// Logging
        services.AddSerilog(c => c.ReadFrom.Configuration(configuration));

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod;
        });

        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(enumerable => enumerable.First());
            options.SwaggerDoc("v1", new OpenApiInfo());
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{typeof(Program).Assembly.GetName().Name}.xml"));
        });

        var app = builder.Build();
// Migration
        app.MigrateDatabase<AppDbContext>();

        app.UseHttpLogging();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

// HealthCheck
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "applications/json";
                var response = new HealthCheckResponse
                {
                    Status = report.Status.ToString(),
                    HealthChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
                    {
                        Status = x.Key, Component = x.Value.Status.ToString(), Description = x.Value.Description
                    }),
                    HealthCheckDuration = report.TotalDuration
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        });
// REST
        app.UseRouting();
        app.MapControllers();

// RUN!
        await app.RunAsync();
    }
}