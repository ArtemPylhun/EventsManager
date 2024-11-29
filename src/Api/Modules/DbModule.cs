using Infrastructure.Persistence;

namespace Api.Modules;

public static class DbModule
{
    public static async Task InitializeDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitializeAsync();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (bool.Parse(config["AllowSeeder"]!))
        {
            await app.SeedRoles();
            await app.SeedLocations();
            await app.SeedCategories();
            await app.SeedUsers();
            await app.SeedEvents();
            await app.SeedAttendances();
            await app.SeedTags();
            await app.SeedEventTags();
        }
    }
}