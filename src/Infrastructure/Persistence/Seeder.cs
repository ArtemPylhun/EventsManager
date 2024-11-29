using Domain;
using Domain.Attendances;
using Domain.Attendencies;
using Domain.Categories;
using Domain.Events;
using Domain.EventsTags;
using Domain.Locations;
using Domain.Profiles;
using Domain.Roles;
using Domain.Tags;
using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class Seeder
{
    public static async Task SeedRoles(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Roles.Any())
        {
            var roles = new[]
            {
                "Admin",
                "User"
            };

            foreach (var role in roles)
                await context.Roles.AddAsync(Role.New(RoleId.New(), role));

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedLocations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Locations.Any())
        {
            var locations = new[]
            {
                ("Main Conference Hall", "123 Tech St", "TechCity", "Techland", 500),
                ("Outdoor Arena", "456 Play St", "Playtown", "Sportsland", 1000),
                ("Virtual Space", "Online", "N/A", "Global", int.MaxValue)
            };

            foreach (var (name, address, city, country, capacity) in locations)
            {
                await context.Locations.AddAsync(Location.New(LocationId.New(), name, address, city, country,
                    capacity));
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedCategories(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new { Name = "Technology", Description = "Events related to technology and innovation" },
                new { Name = "Art", Description = "Creative and artistic events" },
                new { Name = "Science", Description = "Scientific discussions and experiments" },
                new { Name = "Sports", Description = "Athletic competitions and games" },
                new { Name = "Education", Description = "Workshops and educational activities" }
            };

            foreach (var category in categories)
                await context.Categories.AddAsync(Category.New(CategoryId.New(), category.Name, category.Description));

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedUsers(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Users.Any())
        {
            var roles = await context.Roles.ToListAsync();
            Random rand = new Random();
            var users = new[]
            {
                new { UserName = "admin", Email = "admin@example.com", Role = "Admin" },
                new { UserName = "user", Email = "user@example.com", Role = "User" }
            };

            var adminRole = roles.FirstOrDefault(r => r.Title == users[0].Role);
            var userRole = roles.FirstOrDefault(r => r.Title == users[1].Role);

            if (adminRole == null || userRole == null)
                throw new InvalidOperationException("Roles must be seeded before creating Users.");

            var adminProfileId = ProfileId.New();


            var adminUser = User.New(
                UserId.New(),
                users[0].UserName,
                users[0].Email,
                "$2a$10$N4EZAC79lQBnWB2UsFe0j.t1sJsb1Us.2lPlQ5kxXbGElCMfqheIK",
                DateTime.UtcNow,
                adminRole.Id,
                adminProfileId
            );

            var adminProfile = Profile.New(
                adminProfileId,
                $"{adminUser.UserName} Profile",
                DateTime.UtcNow.AddYears(rand.Next(-50, -18)),
                "+123456789",
                "122413 Admin Address"
            );
            //pass Admin!23
            await context.Users.AddAsync(adminUser);
            await context.Profiles.AddAsync(adminProfile);

            var userProfileId = ProfileId.New();

            var userUser = User.New(
                UserId.New(),
                users[1].UserName,
                users[1].Email,
                "$2a$10$ex9P8H./JZ.D4EyJf9XEYO42C8DEvDm6ZViDWV0PSpe3tK7u41r7O",
                DateTime.UtcNow,
                userRole.Id,
                userProfileId
            );
            //pass User!2345
            var userProfile = Profile.New(
                userProfileId,
                $"{userUser.UserName} Profile",
                DateTime.UtcNow.AddYears(rand.Next(-50, -18)),
                "+123456789",
                "1243 User Address"
            );


            await context.Users.AddAsync(userUser);
            await context.Profiles.AddAsync(userProfile);


            await context.SaveChangesAsync();
        }
    }


    public static async Task SeedEvents(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Events.Any())
        {
            var categories = await context.Categories.ToListAsync();
            var locations = await context.Locations.ToListAsync();
            var users = await context.Users.ToListAsync();

            if (!users.Any() || !categories.Any() || !locations.Any())
                throw new InvalidOperationException(
                    "Users, Categories, or Locations must be seeded before creating Events.");

            var events = new[]
            {
                new
                {
                    Title = "AI Conference", Description = "Explore the future of AI",
                    StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(12)
                },
                new
                {
                    Title = "Painting Workshop", Description = "Learn the art of painting",
                    StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(7)
                },
                new
                {
                    Title = "Science Fair", Description = "Showcasing scientific achievements",
                    StartDate = DateTime.UtcNow.AddMonths(1), EndDate = DateTime.UtcNow.AddMonths(1).AddDays(2)
                }
            };

            foreach (var ev in events)
            {
                var randomCategory = categories.OrderBy(c => Guid.NewGuid()).First();
                var randomLocation = locations.OrderBy(l => Guid.NewGuid()).First();
                var randomOrganizer = users.OrderBy(u => Guid.NewGuid()).First();
                await context.Events.AddAsync(Event.New(
                    EventId.New(),
                    ev.Title,
                    ev.Description,
                    ev.StartDate,
                    ev.EndDate,
                    null,
                    randomOrganizer.Id,
                    randomLocation.Id,
                    randomCategory.Id));
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedAttendances(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Attendances.Any())
        {
            var events = await context.Events.ToListAsync();
            var users = await context.Users.ToListAsync();

            if (events.Any() && users.Any())
            {
                foreach (var user in users)
                {
                    var randomEvent = events.OrderBy(_ => Guid.NewGuid()).First();
                    await context.Attendances.AddAsync(Attendance.New(
                        AttendanceId.New(),
                        user.Id,
                        randomEvent.Id,
                        DateTime.UtcNow
                    ));
                }

                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedTags(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Tags.Any())
        {
            var tags = new[]
            {
                "AI",
                "Fitness",
                "Gaming",
                "Mental Health",
                "Programming"
            };

            foreach (var tag in tags)
            {
                await context.Tags.AddAsync(Tag.New(TagId.New(), tag));
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedEventTags(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.EventsTags.Any())
        {
            var events = await context.Events.ToListAsync();
            var tags = await context.Tags.ToListAsync();

            if (events.Any() && tags.Any())
            {
                foreach (var evnt in events)
                {
                    var randomTag = tags.OrderBy(_ => Guid.NewGuid()).First();
                    await context.EventsTags.AddAsync(EventTag.New(EventTagId.New(), evnt.Id, randomTag.Id));
                }

                await context.SaveChangesAsync();
            }
        }
    }
}