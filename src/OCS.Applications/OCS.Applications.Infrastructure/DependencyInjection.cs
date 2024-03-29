using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OCS.Applications.Domain.Repository;
using OCS.Applications.Infrastructure.Contexts;
using OCS.Applications.Infrastructure.Repositories;

namespace OCS.Applications.Infrastructure;

public static class DependencyInjection
{
    public static void RegisterInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterPostgres(configuration);

        services.AddScoped<IApplicationsRepository, ApplicationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepositoryManager, ApplicationsRepositoryManager>();
    }

    private static void RegisterPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var currentAssemblyName = typeof(AppDbContext).Assembly.FullName;
        var dbConnectionString = configuration.GetConnectionString("ApplicationsApi");
        
        services.AddDbContext<AppDbContext>(builder =>
        {
            builder.UseNpgsql(dbConnectionString,
                    b => b.MigrationsAssembly(currentAssemblyName)
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName)
                        .EnableRetryOnFailure());
            
            builder.UseAllCheckConstraints();
        });
    }
}