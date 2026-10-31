using HomeServices.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeServices.Infrastructure.Data;

public static class DatabaseInitializer
{
    /// <summary>
    /// يطبّق الـ Migrations (إن كان ذلك مفعّلاً في الإعدادات) ثم ينشئ الأدوار وحساب المدير.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var configuration = provider.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
        {
            await provider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
        }

        await provider.GetRequiredService<IdentitySeeder>().SeedAsync();
    }
}
