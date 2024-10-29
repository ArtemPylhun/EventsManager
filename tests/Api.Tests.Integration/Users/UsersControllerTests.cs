using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _mainRole = RolesData.MainRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly User _mainUser;
    private readonly User _newUser;

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
        _newUser = UsersData.NewUser(_mainRole.Id);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var userName = "User Name";
        var email = "userCreateName@gmail.com";
        var password = "password";
        var request = new UserDto(
            Id: null,
            Email: email, 
            UserName: userName, 
            Password: password, 
            RoleId: _mainRole.Id.Value, 
            Role: null);

        // Act
        var response = await Client.PostAsJsonAsync("users", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = new UserId(responseUser.Id!.Value);

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(email);
        dbUser.PasswordHash.Should().NotBeEmpty();
        dbUser.RoleId.Value.Should().Be(_mainRole.Id.Value);
        dbUser.ProfileId.Should().NotBe(ProfileId.Empty());
    }
    
    [Fact]
         public async Task ShouldNotCreateUserBecauseNameDuplicated()
         {
             // Arrange
             var userName = "UserName";
             var email = "userTestCreateName@gmail.com";
             var password = "password";
             var request = new UserDto(
                 Id: null,
                 Email: email, 
                 UserName: userName, 
                 Password: password, 
                 RoleId: _mainRole.Id.Value, 
                 Role: null);
     
             // Act
             var response = await Client.PostAsJsonAsync("users", request);
     
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
        var password = "password";
        var request = new UserDto(
            Id: null,
            Email: email, 
            UserName: userName, 
            Password: password, 
            RoleId: _mainRole.Id.Value, 
            Role: null);
     
        // Act
        var response = await Client.PostAsJsonAsync("users", request);
     
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ShouldNotCreateUserBecauseRoleNotFound()
    {
        // Arrange
        var userName = "Create User Name";
        var email = "createUserName@gmail.com";
        var password = "password";
        var request = new UserDto(
            Id: null,
            Email: email, 
            UserName: userName, 
            Password: password, 
            RoleId: RoleId.New().Value, 
            Role: null);
     
        // Act
        var response = await Client.PostAsJsonAsync("users", request);
     
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        var userName = "Create User Name";
        var password = "password";
        var request = new UserUpdateDto(
            Id: _mainUser.Id.Value,
            UserName: userName, 
            Password: password,
            FullName: "Full Name",
            PhoneNumber: "123456789", 
            Address: "city Rivne", 
            BirthDate: DateTime.UtcNow.AddYears(-19));

        // Act
        var response = await Client.PutAsJsonAsync("users", request);

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
        dbProfile.BirthDate.Value.Should().Be(_mainProfile.BirthDate.Value);
    }
    
    /*[Fact]
    public async Task ShouldNotUpdateUserBecauseNotFound()
    {
        var newUserId = Guid.NewGuid();
        // Arrange
        var newFirstName = "ReJohn";
        var newLastName = "ReDoe";
        var request = new UserDto(
            Id: newUserId,
            FirstName: newFirstName,
            LastName: newLastName,
            FullName: null,
            UpdatedAt: null,
            FacultyId: _mainFaculty.Id.Value,
            Faculty: null);

        // Act
        var response = await Client.PutAsJsonAsync("users", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }*/
    
    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Profiles.AddAsync(_mainProfile);
        await Context.Users.AddAsync(_mainUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Profiles.RemoveRange(Context.Profiles);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}