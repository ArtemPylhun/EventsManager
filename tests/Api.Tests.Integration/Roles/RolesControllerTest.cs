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

namespace Api.Tests.Integration.Roles;

public class RolesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _mainRole;
    private readonly User _mainUser;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly Role _noUsersRole = RolesData.NoUsersRole;


    public RolesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainRole = RolesData.UserRole;
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
    }

    [Fact]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var roleName = "Test role name";
        var request = new RoleDto(Guid.NewGuid(), roleName);

        // Act
        var response = await Client.PostAsJsonAsync("roles/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseRole = await response.ToResponseModel<RoleDto>();
        var roleId = new RoleId(responseRole.Id!.Value);

        var dbRole = await Context.Roles.FirstAsync(x => x.Id == roleId);
        dbRole.Should().NotBeNull();
        dbRole.Title.Should().Be(roleName);
    }

    [Fact]
    public async Task ShouldNotCreateRoleBecauseDuplicated()
    {
        // Arrange
        var roleName = _mainRole.Title;
        var request = new RoleDto(Guid.NewGuid(), roleName);

        // Act
        var response = await Client.PostAsJsonAsync("roles/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteRole()
    {
        // Arrange
        var roleId = _noUsersRole.Id;

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseRole = await response.ToResponseModel<RoleDto>();
        var roleIdResponse = new RoleId(responseRole.Id!.Value);

        var dbRole = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleIdResponse);
        dbRole.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteRoleBecauseNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotDeleteRoleBecauseHasUsers()
    {
        // Arrange
        var roleId = _mainRole.Id;

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateRole()
    {
        // Arrange
        var newRoleTitle = "Updated role title";
        var request = new RoleDto(_mainRole.Id.Value, newRoleTitle);

        // Act
        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseRole = await response.ToResponseModel<RoleDto>();
        var roleIdResponse = new RoleId(responseRole.Id.Value);

        var dbRole = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleIdResponse);
        dbRole.Should().NotBeNull();
        dbRole.Title.Should().Be(newRoleTitle);
    }

    [Fact]
    public async Task ShouldNotUpdateRoleBecauseNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var request = new RoleDto(roleId, "Updated role title");

        // Act
        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateRoleBecauseDuplicated()
    {
        // Arrange
        var roleName = _mainRole.Title;
        var request = new RoleDto(_mainRole.Id.Value, roleName);

        // Act
        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Roles.AddAsync(_noUsersRole);
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