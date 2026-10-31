using HomeServices.Application.Constants;
using HomeServices.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Data.Seed;

/// <summary>
/// ينشئ الأدوار وحساب المدير الأول. بيانات المدير تُقرأ من الإعدادات (User Secrets أو متغيرات البيئة)
/// ضمن القسم "SeedAdmin"، ولا تُكتب أبداً في الكود.
/// </summary>
internal sealed class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<IdentitySeeder> logger)
{
    public async Task SeedAsync()
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await SeedAdminAsync();
    }

    private async Task SeedAdminAsync()
    {
        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("SeedAdmin settings are missing; the initial admin account was not created.");
            return;
        }

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = configuration["SeedAdmin:FullName"] ?? "مدير المنصة"
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create the initial admin account: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
        logger.LogInformation("Initial admin account created for {Email}.", email);
    }
}
