using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Categories;
using Domain.Events;
using Domain.Locations;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _mainCategory = CategoriesData.MainCategory;
    private readonly Category _secondaryCategory = CategoriesData.SecondaryCategory;
    private readonly Location _mainLocation = LocationsData.MainLocation;
    private readonly Role _mainRole = RolesData.MainRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly User _mainUser;
    private readonly Event _mainEvent;
    
    public CategoriesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
        _mainEvent = EventsData.MainEvent(_mainUser.Id, _mainLocation.Id, _mainCategory.Id);
    }

    [Fact]
    public async Task ShouldCreateCategory()
    {
        // Arrange
        var categoryName = "Test category name";
        var categoryDescription = "Test category description";
        var request = new CategoryDto(Guid.NewGuid(), categoryName, categoryDescription);

        // Act
        var response = await Client.PostAsJsonAsync("categories", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseCategory = await response.ToResponseModel<CategoryDto>();
        var categoryId = new CategoryId(responseCategory.Id!.Value);
        
        var dbCategory = await Context.Categories.FirstAsync(x => x.Id == categoryId);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(categoryName);
        dbCategory.Description.Should().Be(categoryDescription);
    }
    
    [Fact]
    public async Task ShouldNotCreateCategoryBecauseNameDuplicated()
    {
        // Arrange
        var categoryName = _mainCategory.Name;
        var categoryDescription = "Test category description";
        var request = new CategoryDto(Guid.NewGuid(), categoryName, categoryDescription);

        // Act
        var response = await Client.PostAsJsonAsync("categories", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateCategory()
    {
        // Arrange
        var categoryName = "Updated test category name";
        var categoryDescription = "Updated  category description";
        var request = new CategoryDto(_mainCategory.Id.Value, categoryName, categoryDescription);

        // Act
        var response = await Client.PutAsJsonAsync("categories", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseCategory = await response.ToResponseModel<CategoryDto>();
        var categoryId = new CategoryId(responseCategory.Id!.Value);
        
        var dbCategory = await Context.Categories.FirstAsync(x => x.Id == categoryId);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(categoryName);
        dbCategory.Description.Should().Be(categoryDescription);
    }
    
    [Fact]
    public async Task ShouldNotUpdateCategoryBecauseNameDuplicated()
    {
        // Arrange
        var categoryName = _secondaryCategory.Name;
        var categoryDescription = "Updated  category description";
        var request = new CategoryDto(_mainCategory.Id.Value, categoryName, categoryDescription);

        // Act
        var response = await Client.PutAsJsonAsync("categories", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ShouldNotUpdateCategoryBecauseCategoryNotFound()
    {
        // Arrange
        var categoryName = "Updated category name";
        var categoryDescription = "Updated  category description";
        var request = new CategoryDto(Guid.NewGuid(), categoryName, categoryDescription);

        // Act
        var response = await Client.PutAsJsonAsync("categories", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldDeleteCategory()
    {
        // Arrange
        var categoryId = _secondaryCategory.Id;

        // Act
        var response = await Client.DeleteAsync($"categories/{categoryId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseCategory = await response.ToResponseModel<CategoryDto>();
        responseCategory.Id.Should().Be(categoryId.Value);
        
        var dbCategory = await Context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
        dbCategory.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteCategoryBecauseCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"categories/{categoryId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotDeleteCategoryBecauseCategoryHasEvents()
    {
        // Arrange
        var categoryId = _mainCategory.Id;

        // Act
        var response = await Client.DeleteAsync($"categories/{categoryId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Profiles.AddAsync(_mainProfile);
        await Context.Users.AddAsync(_mainUser);
        await Context.Categories.AddRangeAsync(_mainCategory, _secondaryCategory);
        await Context.Locations.AddAsync(_mainLocation);
        await Context.Events.AddAsync(_mainEvent);
        
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Events.RemoveRange(Context.Events);
        Context.Locations.RemoveRange(Context.Locations);
        Context.Categories.RemoveRange(Context.Categories);
        Context.Users.RemoveRange(Context.Users);
        Context.Profiles.RemoveRange(Context.Profiles);
        Context.Roles.RemoveRange(Context.Roles);
        
        await SaveChangesAsync();
    }
}