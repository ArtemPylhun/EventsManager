using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Roles;
using Domain.Tags;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
namespace Api.Tests.Integration.Tags;

public class TagsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Tag _mainTag = TagsData.MainTag;
    private readonly Tag _secondaryTag = TagsData.SecondaryTag;

    public TagsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldCreateTag()
    {
        // Arrange
        var tagName = "Test tag name";
        var request = new TagDto(Guid.NewGuid(), tagName);

        // Act
        var response = await Client.PostAsJsonAsync("tags", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseTag = await response.ToResponseModel<TagDto>();
        var tagId = new TagId(responseTag.Id!.Value);

        var dbTag = await Context.Tags.FirstAsync(x => x.Id == tagId);
        dbTag.Should().NotBeNull();
        dbTag.Title.Should().Be(tagName);
    }

    [Fact]
    public async Task ShouldNotCreateTagBecauseDuplicated()
    {
        // Arrange
        var tagName = _mainTag.Title;
        var request = new TagDto(Guid.NewGuid(), tagName);

        // Act
        var response = await Client.PostAsJsonAsync("tags", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteTag()
    {
        // Arrange
        var tagId = _mainTag.Id;

        // Act
        var response = await Client.DeleteAsync($"tags/{tagId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseTag = await response.ToResponseModel<TagDto>();
        var tagIdResponse = new TagId(responseTag.Id!.Value);

        var dbTag = await Context.Tags.FirstOrDefaultAsync(x => x.Id == tagIdResponse);
        dbTag.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteTagBecauseNotFound()
    {
        // Arrange
        var tagId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"tags/{tagId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    [Fact]
    public async Task ShouldUpdateTag()
    {
        // Arrange
        var tagName = "Updated tag title";
        var request = new TagDto(_mainTag.Id.Value, tagName);

        // Act
        var response = await Client.PutAsJsonAsync("tags", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseTag = await response.ToResponseModel<TagDto>();
        var tagId = new TagId(responseTag.Id!.Value);

        var dbTag = await Context.Tags.FirstAsync(x => x.Id == tagId);
        dbTag.Should().NotBeNull();
        dbTag.Title.Should().Be(tagName);
    }

    [Fact]
    public async Task ShouldNotUpdateTagBecauseTitleDuplicated()
    {
        // Arrange
        var tagName = _secondaryTag.Title;
        var request = new TagDto(_mainTag.Id.Value, tagName);

        // Act
        var response = await Client.PutAsJsonAsync("tags", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task ShouldNotUpdateTagBecauseNotFound()
    {
        // Arrange
        var tagName = "Updated tag title";
        var request = new TagDto(Guid.NewGuid(), tagName);

        // Act
        var response = await Client.PutAsJsonAsync("tags", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    public async Task InitializeAsync()
    {
        await Context.Tags.AddRangeAsync(_mainTag, _secondaryTag);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Tags.RemoveRange(Context.Tags);
        await SaveChangesAsync();
    }
}