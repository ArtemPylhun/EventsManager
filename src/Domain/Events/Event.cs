using Domain.Attendances;
using Domain.Categories;
using Domain.EventsTags;
using Domain.Locations;
using Domain.Tags;
using Domain.Users;

namespace Domain.Events;

public class Event
{
    public EventId Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public UserId OrganizerId { get; }
    public User? Organizer { get; }
    public LocationId LocationId { get; private set; }
    public Location? Location { get; }
    public CategoryId CategoryId { get; private set; }
    public Category? Category { get; }

    public ICollection<EventTag> EventsTags { get; set; } = new List<EventTag>();

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    private Event(EventId id, string title, string description, DateTime startDate, DateTime endDate, UserId organizerId, LocationId locationId, CategoryId categoryId)
    {
        Id = id;
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        OrganizerId = organizerId;
        LocationId = locationId;
        CategoryId = categoryId;
    }
    
    public static Event New(EventId id, string title, string description, DateTime startDate, DateTime endDate, UserId organizerId, LocationId locationId, CategoryId categoryId ) => 
        new(id, title,description,startDate,endDate, organizerId, locationId, categoryId);

    public void UpdateDetails(string title, string description, DateTime startDate, DateTime endDate, LocationId locationId, CategoryId categoryId)
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        LocationId = locationId;
        CategoryId = categoryId;
    }
}