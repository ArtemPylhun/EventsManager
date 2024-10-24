using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<CategoryRepository>());
        services.AddScoped<ICategoryQueries>(provider => provider.GetRequiredService<CategoryRepository>());

        services.AddScoped<TagRepository>();
        services.AddScoped<ITagRepository>(provider => provider.GetRequiredService<TagRepository>());
        services.AddScoped<ITagQueries>(provider => provider.GetRequiredService<TagRepository>());

        services.AddScoped<EventRepository>();
        services.AddScoped<IEventRepository>(provider => provider.GetRequiredService<EventRepository>());
        services.AddScoped<IEventQueries>(provider => provider.GetRequiredService<EventRepository>());

        services.AddScoped<EventTagRepository>();
        services.AddScoped<IEventTagRepository>(provider => provider.GetRequiredService<EventTagRepository>());
        services.AddScoped<IEventTagQueries>(provider => provider.GetRequiredService<EventTagRepository>());
    }
}