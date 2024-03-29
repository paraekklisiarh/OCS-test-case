using Microsoft.EntityFrameworkCore;

namespace OCS.Applications.Api;

/// <summary>
/// Набор методов расширения для накатывания миграций на базу данных
/// </summary>
public static class MigrateExtension
{
    /// <summary>
    /// Накатывает миграции на существующие базы
    /// </summary>
    /// <param name="webApplication">Веб приложение</param>
    /// <typeparam name="TContext">Контекст БД</typeparam>
    internal static IHost MigrateDatabase<TContext>(this IHost webApplication) where TContext : DbContext
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetService<TContext>();
        
        context?.Database.Migrate();

        return webApplication;
    }
}