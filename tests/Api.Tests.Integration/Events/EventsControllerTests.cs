using Domain.Categories;
using Domain.Events;
using Domain.Locations;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Events;

public class EventsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _mainCategory = CategoriesData.MainCategory;
    private readonly Location _mainLocation = LocationsData.MainLocation;
    private readonly Role _mainRole = RolesData.MainRole;
    private readonly Profile _mainProfile = ProfilesData.MainProfile;
    private readonly User _mainUser;
    private readonly Event _mainEvent;
    
    public EventsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainRole.Id, _mainProfile.Id);
        _mainEvent = EventsData.MainEvent(_mainUser.Id, _mainLocation.Id, _mainCategory.Id);
    }

    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await Context.Profiles.AddAsync(_mainProfile);
        await Context.Users.AddAsync(_mainUser);
        await Context.Categories.AddAsync(_mainCategory);
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