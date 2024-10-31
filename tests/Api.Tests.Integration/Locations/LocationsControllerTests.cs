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

namespace Api.Tests.Integration.Locations;

public class LocationsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Location _mainLocation = LocationsData.MainLocation;
    private readonly Location _secondaryLocation = LocationsData.SecondaryLocation;
    private readonly Category _mainCategory = CategoriesData.MainCategory;
    private readonly Role _mainRole = RolesData.UserRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly User _mainUser;
    private readonly Event _mainEvent;

    public LocationsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
        _mainEvent = EventsData.MainEvent(_mainUser.Id, _mainLocation.Id, _mainCategory.Id);
    }

    [Fact]
    public async Task ShouldCreateLocation()
    {
        // Arrange
        var locationName = "Test location name";
        var locationAddress = "Test location address";
        var locationCity = "Test location city";
        var locationCountry = "Test location country";
        var locationCapacity = 5;
        var request = new LocationDto(
            Guid.NewGuid(),
            locationName,
            locationAddress,
            locationCity,
            locationCountry,
            locationCapacity);

        // Act
        var response = await Client.PostAsJsonAsync("locations", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseLocation = await response.ToResponseModel<LocationDto>();
        var locationId = new LocationId(responseLocation.Id!.Value);

        var dbLocation = await Context.Locations.FirstAsync(x => x.Id == locationId);
        dbLocation.Should().NotBeNull();
        dbLocation.Name.Should().Be(locationName);
        dbLocation.Address.Should().Be(locationAddress);
        dbLocation.City.Should().Be(locationCity);
        dbLocation.Country.Should().Be(locationCountry);
        dbLocation.Capacity.Should().Be(locationCapacity);
    }

    [Fact]
    public async Task ShouldNotCreateLocationBecauseLocationExists()
    {
        // Arrange
        var locationName = _mainLocation.Name;
        var locationAddress = _mainLocation.Address;
        var locationCity = _mainLocation.City;
        var locationCountry = _mainLocation.Country;
        var locationCapacity = 5;
        var request = new LocationDto(
            Guid.NewGuid(),
            locationName,
            locationAddress,
            locationCity,
            locationCountry,
            locationCapacity);

        // Act
        var response = await Client.PostAsJsonAsync("locations", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateLocation()
    {
        // Arrange
        var locationName = "New location name";
        var locationAddress = "New location address";
        var locationCity = "New location city";
        var locationCountry = "New location country";
        var locationCapacity = 10;
        var request = new LocationDto(
            _mainLocation.Id.Value,
            locationName,
            locationAddress,
            locationCity,
            locationCountry,
            locationCapacity);

        // Act
        var response = await Client.PutAsJsonAsync("locations", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseLocation = await response.ToResponseModel<LocationDto>();
        var locationId = new LocationId(responseLocation.Id!.Value);

        var dbLocation = await Context.Locations.FirstAsync(x => x.Id == locationId);
        dbLocation.Should().NotBeNull();
        dbLocation.Name.Should().Be(locationName);
        dbLocation.Address.Should().Be(locationAddress);
        dbLocation.City.Should().Be(locationCity);
        dbLocation.Country.Should().Be(locationCountry);
        dbLocation.Capacity.Should().Be(locationCapacity);
    }

    [Fact]
    public async Task ShouldNotUpdateLocationBecauseSuchLocationExists()
    {
        // Arrange
        var locationName = _secondaryLocation.Name;
        var locationAddress = _secondaryLocation.Address;
        var locationCity = _secondaryLocation.City;
        var locationCountry = _secondaryLocation.Country;
        var locationCapacity = _secondaryLocation.Capacity;
        var request = new LocationDto(
            _mainLocation.Id.Value,
            locationName,
            locationAddress,
            locationCity,
            locationCountry,
            locationCapacity);

        // Act
        var response = await Client.PutAsJsonAsync("locations", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateLocationBecauseLocationNotFound()
    {
        // Arrange
        var locationName = "New location name";
        var locationAddress = "New location address";
        var locationCity = "New location city";
        var locationCountry = "New location country";
        var locationCapacity = 10;
        var request = new LocationDto(
            Guid.NewGuid(),
            locationName,
            locationAddress,
            locationCity,
            locationCountry,
            locationCapacity);

        // Act
        var response = await Client.PutAsJsonAsync("locations", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteLocation()
    {
        // Arrange
        var locationId = _secondaryLocation.Id;

        // Act
        var response = await Client.DeleteAsync($"locations/{locationId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseLocation = await response.ToResponseModel<CategoryDto>();
        responseLocation.Id.Should().Be(locationId.Value);

        var dbCategory = await Context.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
        dbCategory.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteLocationBecauseLocationNotFound()
    {
        // Arrange
        var locationId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"locations/{locationId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotDeleteLocationBecauseLocationHasEvents()
    {
        // Arrange
        var locationId = _mainLocation.Id;

        // Act
        var response = await Client.DeleteAsync($"locations/{locationId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Profiles.AddAsync(_mainProfile);
        await Context.Users.AddAsync(_mainUser);
        await Context.Categories.AddAsync(_mainCategory);
        await Context.Locations.AddRangeAsync(_mainLocation, _secondaryLocation);
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