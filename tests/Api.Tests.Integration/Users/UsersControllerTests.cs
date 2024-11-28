using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Attendances;
using Domain.Attendencies;
using Domain.Categories;
using Domain.Events;
using Domain.Locations;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _userRole = RolesData.UserRole;
    private readonly Role _adminRole = RolesData.AdminRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly Profile _mainProfileTwo = ProfilesData.MainProfileTwo;
    private readonly Profile _mainProfileThree = ProfilesData.MainProfileThree;
    private readonly Profile _newProfile = ProfilesData.NewProfile;
    private readonly Location _mainLocation = LocationsData.MainLocation;
    private readonly Category _mainCategory = CategoriesData.MainCategory;
    private readonly User _mainUser;
    private readonly User _newUser;
    private readonly User _mainUserTwo;
    private readonly User _mainUserThree;
    private readonly Event _mainEvent;
    private readonly Event _secondaryEvent;
    private readonly Attendance _mainAttendance;
    private readonly Attendance _pastAttendance;
    private readonly Attendance _futureAttendance;


    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_userRole.Id, _mainProfile.Id);
        _mainUserTwo = UsersData.MainUserTwo(_userRole.Id, _mainProfileTwo.Id);
        _mainUserThree = UsersData.MainUserThree(_userRole.Id, _mainProfileThree.Id);
        _newUser = UsersData.NewUser(_userRole.Id, _newProfile.Id);
        _secondaryEvent = EventsData.SecondaryEvent(_mainUserThree.Id, _mainLocation.Id, _mainCategory.Id);
        _mainEvent = EventsData.MainEvent(_mainUserThree.Id, _mainLocation.Id, _mainCategory.Id);
        _pastAttendance = AttendancesData.PastAttendance(_mainUserTwo.Id, _mainEvent.Id,
            DateTime.UtcNow.AddYears(-2));
        _futureAttendance =
            AttendancesData.FutureAttendance(_mainUserThree.Id, _mainEvent.Id, DateTime.UtcNow.AddYears(2));
        _mainAttendance = AttendancesData.MainAttendance(_mainUser.Id, _mainEvent.Id, DateTime.UtcNow);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var userName = "User Name";
        var email = "userCreateName@gmail.com";
        var password = "Admin!23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password,
            RoleId: _userRole.Id.Value,
            Role: null,
            Profile: null);

        // Act
        var response = await Client.PostAsJsonAsync("users/register", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = new UserId(responseUser.Id!.Value);

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(email);
        dbUser.PasswordHash.Should().NotBeEmpty();
        dbUser.RoleId.Value.Should().Be(_userRole.Id.Value);
        dbUser.ProfileId.Should().NotBe(ProfileId.Empty());
    }

    [Fact]
    public async Task ShouldNotCreateUserBecauseNameDuplicated()
    {
        // Arrange
        var userName = "UserName";
        var email = "userTestCreateName@gmail.com";
        var password = "Admin!23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password,
            RoleId: _userRole.Id.Value,
            Role: null,
            Profile: null);

        // Act
        var response = await Client.PostAsJsonAsync("users/register", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateUserBecauseEmailDuplicated()
    {
        // Arrange
        var userName = "User Name";
        var email = "userName@gmail.com";
        var password = "Admin!23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password,
            RoleId: _userRole.Id.Value,
            Role: null,
            Profile: null);

        // Act
        var response = await Client.PostAsJsonAsync("users/register", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        var userName = "Create User Name";
        var password = "Admin!23";
        var request = new UserUpdateDto(
            Id: _mainUser.Id.Value,
            UserName: userName,
            Password: password,
            FullName: "Full Name",
            PhoneNumber: "123456789",
            Address: "city Rivne",
            BirthDate: DateTime.UtcNow.AddYears(-19));

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserUpdateDto>();
        var userId = new UserId(responseUser.Id);

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        var dbProfile = await Context.Profiles.FirstAsync(x => x.Id == _mainProfile.Id);
        dbUser.Id.Value.Should().Be(_mainUser.Id.Value);
        dbUser.UserName.Should().Be(userName);
        dbUser.PasswordHash.Should().Be(password);
        dbProfile.FullName.Should().Be(_mainProfile.FullName);
        dbProfile.PhoneNumber.Should().Be(_mainProfile.PhoneNumber);
        dbProfile.Address.Should().Be(_mainProfile.Address);
        dbProfile.BirthDate.Should().BeSameDateAs(_mainProfile.BirthDate.Value);
    }

    [Fact]
    public async Task ShouldNotUpdateUserBecauseUserNotFound()
    {
        var userId = Guid.NewGuid();
        var userName = "Update User Name";
        var email = "updateUserName@gmail.com";
        var password = "Admin!23";
        var request = new UserUpdateDto(
            Id: userId,
            UserName: userName,
            Password: password,
            FullName: "Full Name",
            PhoneNumber: "123456789",
            Address: "city Rivne",
            BirthDate: DateTime.UtcNow.AddYears(-19));

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateUserBecauseUserNameDuplicated()
    {
        // Arrange
        var userName = "UserName";
        var password = "Admin!23";
        var request = new UserUpdateDto(
            Id: _mainUser.Id.Value,
            UserName: userName,
            Password: password,
            FullName: "Full Name",
            PhoneNumber: "123456789",
            BirthDate: DateTime.UtcNow.AddYears(-19),
            Address: "city Rivne");

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        var userId = _mainUser.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.IsSuccessStatusCode.Should().BeTrue($"Response: {await response.Content.ReadAsStringAsync()}");
        var responseUser = await response.ToResponseModel<UserDto>();
        responseUser.Id.Should().Be(userId);

        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == new UserId(responseUser.Id.Value));
        dbUser.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserBecauseNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldLoginUser()
    {
        // Arrange
        var email = _mainUser.Email;
        var password = "Admin!23";
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var result = await response.ToResponseModel();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotLoginUserBecauseNotFoundByEmail()
    {
        // Arrange
        var email = "notFoundEmail@gmail.com";
        var password = "Admin!23";
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldNotLoginUserBecausePasswordNotMatch()
    {
        // Arrange
        var email = _mainUser.Email;
        var password = "wrongPassword!23";
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldChangeUserRole()
    {
        // Arrange
        var existingRole = _adminRole;
        var request = new UpdateUserRoleDto(
            UserId: _mainUser.Id.Value,
            RoleId: existingRole.Id.Value);

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var dbUser = await Context.Users.FirstAsync(x => x.Id == _mainUser.Id);
        dbUser.RoleId.Value.Should().Be(existingRole.Id.Value);
    }

    [Fact]
    public async Task ShouldNotChangeUserRoleBecauseUserNotFound()
    {
        // Arrange
        var existingRole = _adminRole;
        var request = new UpdateUserRoleDto(UserId: Guid.NewGuid(),
            RoleId: existingRole.Id.Value);

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotChangeUserRoleBecauseRoleNotFound()
    {
        // Arrange
        var request = new UpdateUserRoleDto(
            UserId: _mainUser.Id.Value,
            RoleId: Guid.NewGuid());

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldRegisterUserToEventSuccessfully()
    {
        // Arrange
        var userId = _newUser.Id.Value;
        var eventId = _mainEvent.Id.Value;

        // Act
        var response = await Client.PostAsync($"users/registerToEvent/{eventId}?userId={userId}", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseAttendance = await response.ToResponseModel<AttendanceRegisterDto>();
        var attendanceId = new AttendanceId(responseAttendance.Id!.Value);

        var dbAttendance = await Context.Attendances.FirstAsync(x => x.Id == attendanceId);
        dbAttendance.UserId.Value.Should().Be(userId);
        dbAttendance.EventId.Value.Should().Be(eventId);
    }

    [Fact]
    public async Task ShouldNotRegisterUserToEventBecauseUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = _mainEvent.Id.Value;

        // Act
        var response = await Client.PostAsync($"users/registerToEvent/{eventId}?userId={userId}", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotRegisterUserToEventBecauseEventNotFound()
    {
        // Arrange
        var userId = _mainUser.Id.Value;
        var eventId = Guid.NewGuid();

        // Act
        var response = await Client.PostAsync($"users/registerToEvent/{eventId}?userId={userId}", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotRegisterUserToEventBecauseAlreadyRegistered()
    {
        // Arrange
        var userId = _mainUser.Id.Value;
        var eventId = _mainEvent.Id.Value;

        // Act
        var response = await Client.PostAsync($"users/registerToEvent/{eventId}?userId={userId}", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUnregisterUserFromEventSuccessfully()
    {
        // Arrange
        var userId = _mainUser.Id.Value;
        var eventId = _mainEvent.Id.Value;
        var attendanceId = _mainAttendance.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"users/unregisterFromEvent/{eventId}?userId={userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.IsSuccessStatusCode.Should().BeTrue($"Response: {await response.Content.ReadAsStringAsync()}");
        var responseAttendance = await response.ToResponseModel<AttendanceUnregisterDto>();
        responseAttendance.Id.Should().Be(attendanceId);

        var dbAttendance =
            await Context.Attendances.FirstOrDefaultAsync(x => x.Id == new AttendanceId(responseAttendance.Id.Value));
        dbAttendance.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotUnregisterUserFromEventBecauseUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = _mainEvent.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"users/unregisterFromEvent/{eventId}?userId={userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUnregisterUserFromEventBecauseEventNotFound()
    {
        // Arrange
        var userId = _mainUser.Id.Value;
        var eventId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"users/unregisterFromEvent/{eventId}?userId={userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUnregisterUserFromEventBecauseNotRegistered()
    {
        // Arrange
        var userId = _mainUser.Id.Value;
        var eventId = _secondaryEvent.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"users/unregisterFromEvent/{eventId}?userId={userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_mainCategory);
        await Context.Locations.AddAsync(_mainLocation);
        await Context.Profiles.AddRangeAsync(_newProfile, _mainProfile, _mainProfileTwo, _mainProfileThree);
        await Context.Roles.AddRangeAsync(_userRole, _adminRole);
        await Context.Users.AddRangeAsync(_newUser, _mainUser, _mainUserTwo, _mainUserThree);
        await Context.Events.AddRangeAsync(_mainEvent, _secondaryEvent);
        await Context.Attendances.AddRangeAsync(_mainAttendance, _pastAttendance, _futureAttendance);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Attendances.RemoveRange(Context.Attendances);
        Context.Profiles.RemoveRange(Context.Profiles);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Events.RemoveRange(Context.Events);
        Context.Categories.RemoveRange(Context.Categories);
        Context.Locations.RemoveRange(Context.Locations);

        await SaveChangesAsync();
    }
}