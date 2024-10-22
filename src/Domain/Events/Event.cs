using Domain.Locations;

namespace Domain.Events;

public class Event
{
    public EventId Id { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    /*public UserId UserId { get; set; }
    public User User { get; set; }*/
    public LocationId LocationId { get; set; }
    public Location Location { get; set; }
    /*public CategoryId CategoryId { get; set; }
    public Category Category { get; set; }*/
}