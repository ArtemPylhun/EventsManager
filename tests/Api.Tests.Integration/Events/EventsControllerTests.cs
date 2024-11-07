using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Categories;
using Domain.Events;
using Domain.EventsTags;
using Domain.Locations;
using Domain.Profiles;
using Domain.Roles;
using Domain.Tags;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Events;

public class EventsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _mainCategory = CategoriesData.MainCategory;
    private readonly Location _mainLocation = LocationsData.MainLocation;
    private readonly Role _mainRole = RolesData.UserRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly Tag _secondaryTag = TagsData.SecondaryTag;
    private readonly Tag _mainTag = TagsData.MainTag;
    private readonly User _mainUser;
    private readonly Event _mainEvent;
    private readonly Event _secondaryEvent;
    private readonly EventTag _mainEventTag;

    public EventsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
        _mainEvent = EventsData.MainEvent(_mainUser.Id, _mainLocation.Id, _mainCategory.Id);
        _secondaryEvent = EventsData.SecondaryEvent(_mainUser.Id, _mainLocation.Id, _mainCategory.Id);
        _mainEventTag = EventTagsData.New(_mainEvent.Id, _mainTag.Id);
    }

    [Fact]
    public async Task ShouldCreateEvent()
    {
        // Arrange
        var eventTitle = "Test event title";
        var eventDescription = "Test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(1);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(10);
        var organizerId = _mainUser.Id;
        var locationId = _mainLocation.Id;
        var categoryId = _mainCategory.Id;
        var request = new MultipartFormDataContent
        {
            { new StringContent(eventTitle), "title" },
            { new StringContent(eventDescription), "description" },
            { new StringContent(startDate.ToString()), "startDate" },
            { new StringContent(endDate.ToString()), "endDate" },
            { new StringContent(organizerId.Value.ToString()), "organizerId" },
            { new StringContent(locationId.Value.ToString()), "locationId" },
            { new StringContent(categoryId.Value.ToString()), "categoryId" },
            { new StringContent(_mainTag.Id.Value.ToString()), "tagsIds" }
        };
        request.Add(new StreamContent(new MemoryStream(new byte[] { 0 })), "image", "test.png");
        
        // Act
        var response = await Client.PostAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseEvent = await response.ToResponseModel<EventDto>();
        var eventId = new EventId(responseEvent.Id);

        var dbEvent = await Context.Events.FirstAsync(x => x.Id == eventId);
        dbEvent.Should().NotBeNull();
        dbEvent.Title.Should().Be(eventTitle);
        dbEvent.Description.Should().Be(eventDescription);
        dbEvent.StartDate.ToString("F").Should().Be(startDate.ToString("F"));
        dbEvent.EndDate.ToString("F").Should().Be(endDate.ToString("F"));
        dbEvent.OrganizerId.Should().Be(organizerId);
        dbEvent.LocationId.Should().Be(locationId);
        dbEvent.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task ShouldNotCreateEventBecauseCategoryAlreadyExists()
    {
        // Arrange
        var eventTitle = _mainEvent.Title;
        var eventDescription = _mainEvent.Description;
        var startDate = _mainEvent.StartDate;
        var endDate = _mainEvent.EndDate;
        var organizerId = _mainEvent.OrganizerId;
        var locationId = _mainEvent.LocationId;
        var categoryId = _mainEvent.CategoryId;
        var request = new EventCreateDto(eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId.Value,
            categoryId.Value,
            new[] { _mainTag.Id.Value });

        // Act
        var response = await Client.PostAsJsonAsync("events", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateEventBecauseOrganizerNotFound()
    {
        // Arrange
        var eventTitle = _mainEvent.Title;
        var eventDescription = _mainEvent.Description;
        var startDate = _mainEvent.StartDate;
        var endDate = _mainEvent.EndDate;
        var organizerId = Guid.NewGuid();
        var locationId = _mainEvent.LocationId;
        var categoryId = _mainEvent.CategoryId;
        var request = new EventCreateDto(eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId,
            locationId.Value,
            categoryId.Value,
            new[] { _mainTag.Id.Value });

        // Act
        var response = await Client.PostAsJsonAsync("events", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateEventBecauseLocationNotFound()
    {
        // Arrange
        var eventTitle = _mainEvent.Title;
        var eventDescription = _mainEvent.Description;
        var startDate = _mainEvent.StartDate;
        var endDate = _mainEvent.EndDate;
        var organizerId = _mainEvent.OrganizerId;
        var locationId = Guid.NewGuid();
        var categoryId = _mainEvent.CategoryId;
        var request = new EventCreateDto(eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId,
            categoryId.Value,
            new[] { _mainTag.Id.Value });

        // Act
        var response = await Client.PostAsJsonAsync("events", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateEventBecauseCategoryNotFound()
    {
        // Arrange
        var eventTitle = _mainEvent.Title;
        var eventDescription = _mainEvent.Description;
        var startDate = _mainEvent.StartDate;
        var endDate = _mainEvent.EndDate;
        var organizerId = _mainEvent.OrganizerId;
        var locationId = _mainEvent.LocationId;
        var categoryId = Guid.NewGuid();
        var request = new EventCreateDto(eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId.Value,
            categoryId,
            new[] { _mainTag.Id.Value });

        // Act
        var response = await Client.PostAsJsonAsync("events", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateEvent()
    {
        // Arrange
        var eventTitle = "Updated test event title";
        var eventDescription = "Updated test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(2);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(20);
        var organizerId = _mainUser.Id;
        var locationId = _mainLocation.Id;
        var categoryId = _mainCategory.Id;
        var request = new EventUpdateDto(
            _mainEvent.Id.Value,
            eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId.Value,
            categoryId.Value,
            new[] { _secondaryTag.Id.Value });

        // Act
        var response = await Client.PutAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseEvent = await response.ToResponseModel<EventDto>();
        var eventId = new EventId(responseEvent.Id);

        var dbEvent = await Context.Events.FirstAsync(x => x.Id == eventId);
        dbEvent.Should().NotBeNull();
        dbEvent.Title.Should().Be(eventTitle);
        dbEvent.Description.Should().Be(eventDescription);
        dbEvent.StartDate.ToString("F").Should().Be(startDate.ToString("F"));
        dbEvent.EndDate.ToString("F").Should().Be(endDate.ToString("F"));
        dbEvent.OrganizerId.Should().Be(organizerId);
        dbEvent.LocationId.Should().Be(locationId);
        dbEvent.CategoryId.Should().Be(categoryId);
    }


    [Fact]
    public async Task ShouldNotUpdateEventBecauseEventNotFound()
    {
        // Arrange
        var eventTitle = "Updated test event title";
        var eventDescription = "Updated test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(2);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(20);
        var organizerId = _mainUser.Id;
        var locationId = _mainLocation.Id;
        var categoryId = _mainCategory.Id;
        var request = new EventUpdateDto(
            Guid.NewGuid(),
            eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId.Value,
            categoryId.Value,
            new[] { _secondaryTag.Id.Value });

        // Act
        var response = await Client.PutAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateEventBecauseOrganizerNotFound()
    {
        // Arrange
        var eventTitle = "Updated test event title";
        var eventDescription = "Updated test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(2);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(20);
        var organizerId = Guid.NewGuid();
        var locationId = _mainLocation.Id;
        var categoryId = _mainCategory.Id;
        var request = new EventUpdateDto(
            _mainEvent.Id.Value,
            eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId,
            locationId.Value,
            categoryId.Value,
            new[] { _secondaryTag.Id.Value });

        // Act
        var response = await Client.PutAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateEventBecauseLocationNotFound()
    {
        // Arrange
        var eventTitle = "Updated test event title";
        var eventDescription = "Updated test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(2);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(20);
        var organizerId = _mainUser.Id;
        var locationId = Guid.NewGuid();
        var categoryId = _mainCategory.Id;
        var request = new EventUpdateDto(
            _mainEvent.Id.Value,
            eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId,
            categoryId.Value,
            new[] { _secondaryTag.Id.Value });

        // Act
        var response = await Client.PutAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateEventBecauseCategoryNotFound()
    {
        // Arrange
        var eventTitle = "Updated test event title";
        var eventDescription = "Updated test event description";
        var startDate = DateTime.UtcNow + TimeSpan.FromHours(2);
        var endDate = DateTime.UtcNow + TimeSpan.FromHours(20);
        var organizerId = _mainUser.Id;
        var locationId = _mainLocation.Id;
        var categoryId = Guid.NewGuid();
        var request = new EventUpdateDto(
            _mainEvent.Id.Value,
            eventTitle,
            eventDescription,
            startDate,
            endDate,
            null,
            organizerId.Value,
            locationId.Value,
            categoryId,
            new[] { _secondaryTag.Id.Value });

        // Act
        var response = await Client.PutAsJsonAsync("events", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteEvent()
    {
        // Arrange
        var eventId = _secondaryEvent.Id;

        // Act
        var response = await Client.DeleteAsync($"events/{eventId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseEvent = await response.ToResponseModel<EventDto>();
        responseEvent.Id.Should().Be(eventId.Value);

        var dbEvent = await Context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        dbEvent.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteEventBecauseEventNotFound()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"events/{eventId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Profiles.AddAsync(_mainProfile);
        await Context.Users.AddAsync(_mainUser);
        await Context.Categories.AddAsync(_mainCategory);
        await Context.Locations.AddAsync(_mainLocation);
        await Context.Events.AddRangeAsync(_mainEvent, _secondaryEvent);
        await Context.Tags.AddRangeAsync(_mainTag, _secondaryTag);
        await Context.EventsTags.AddAsync(_mainEventTag);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.EventsTags.RemoveRange(Context.EventsTags);
        Context.Tags.RemoveRange(Context.Tags);
        Context.Events.RemoveRange(Context.Events);
        Context.Locations.RemoveRange(Context.Locations);
        Context.Categories.RemoveRange(Context.Categories);
        Context.Users.RemoveRange(Context.Users);
        Context.Profiles.RemoveRange(Context.Profiles);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}