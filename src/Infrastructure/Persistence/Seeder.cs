/*using Domain;
using Domain.Courses;
using Domain.Faculties;
using Domain.JoinEntities;
using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Infrastructure.Persistence;

public static class Seeder
{
    private static HttpClient _httpClient = new();

    public static async Task SeedUsers(this IApplicationBuilder app, int count)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Users.Any())
        {
            var randomUserResponse = await _httpClient.GetAsync($"https://randomuser.me/api/?inc=name&results={count}");

            if (randomUserResponse.IsSuccessStatusCode)
            {
                var randomUserResponseContent = await randomUserResponse.Content.ReadAsStringAsync();

                var randomUser = JsonConvert.DeserializeObject<dynamic>(randomUserResponseContent);

                var faculties = await context.Faculties.ToListAsync();

                if (randomUser is not null)
                    foreach (var user in randomUser.results)
                        await context.Users.AddAsync(User.New(UserId.New(), user.name.first, user.name.last, faculties.OrderBy(f => Guid.NewGuid()).First().Id));
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedFaculties(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Faculties.Any())
        {
            var faculties = new[]
            {
                "Faculty of Computer Science",
                "Faculty of Economics",
                "Faculty of Physics",
                "Faculty of Mathematics",
                "Faculty of Philosophy",
                "Faculty of Biology",
                "Faculty of Chemistry",
                "Faculty of History",
                "Faculty of Law",
                "Faculty of Linguistics",
                "Faculty of Pedagogy",
                "Faculty of Psychology",
                "Faculty of Sociology"
            };

            foreach (var faculty in faculties)
                await context.Faculties.AddAsync(Faculty.New(FacultyId.New(), faculty));

            await context.SaveChangesAsync();
        }
    }


    public static async Task SeedCoursesAndUserCourses(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Courses.Any())
        {
            var courses = new[]
            {
                "Algorithms",
                "Database Management",
                "Computer Networks",
                "Operating Systems",
                "Programming",
                "Calculus",
                "Linear Algebra",
                "Statistics",
                "Physics",
                "Chemistry",
                "Biology",
                "History"
            };

            foreach (var course in courses)
                await context.Courses.AddAsync(Course.New(CourseId.New(), course));

            await context.SaveChangesAsync();

            var randomUsers = await context.Users.ToListAsync();
            var randomCourses = await context.Courses.ToListAsync();

            foreach (var user in randomUsers)
            {
                var randomCourse = randomCourses.OrderBy(c => Guid.NewGuid()).First();
                await context.UserCourses.AddAsync(UserCourse.New(user.Id, randomCourse.Id));
            }

            await context.SaveChangesAsync();
        }
    }
}*/

