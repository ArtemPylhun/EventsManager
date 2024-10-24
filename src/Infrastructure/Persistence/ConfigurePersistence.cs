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
        
        services.AddScoped<AttendanceRepository>();
        services.AddScoped<IAttendanceRepository>(provider => provider.GetRequiredService<AttendanceRepository>());
        services.AddScoped<IAttendanceQueries>(provider => provider.GetRequiredService<AttendanceRepository>());
        
        services.AddScoped<LocationRepository>();
        services.AddScoped<ILocationRepository>(provider => provider.GetRequiredService<LocationRepository>());
        services.AddScoped<ILocationQueries>(provider => provider.GetRequiredService<LocationRepository>());
        
        services.AddScoped<ProfileRepository>();
        services.AddScoped<IProfileRepository>(provider => provider.GetRequiredService<ProfileRepository>());
        services.AddScoped<IProfileQueries>(provider => provider.GetRequiredService<ProfileRepository>());
        
        services.AddScoped<RoleRepository>();
        services.AddScoped<IRoleRepository>(provider => provider.GetRequiredService<RoleRepository>());
        services.AddScoped<IRoleQueries>(provider => provider.GetRequiredService<RoleRepository>());
        
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());

    }
}