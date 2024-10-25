using Domain.Events;
using Domain.Tags;

namespace Domain.EventsTags;

public class EventTag
{
    public EventTagId Id { get; }
    public EventId EventId { get; }
    public Event? Event { get; }
    public TagId TagId { get;}
    public Tag? Tag { get; }

    private EventTag(EventTagId id, EventId eventId, TagId tagId)
    {
        Id = id;
        EventId = eventId;
        TagId = tagId;
    }
    
    public static EventTag New(EventTagId eventTagId, EventId eventId, TagId tagId) =>
    new(eventTagId, eventId, tagId);
}