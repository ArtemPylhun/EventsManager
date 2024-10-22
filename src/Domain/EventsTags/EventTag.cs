using Domain.Events;
using Domain.Tags;

namespace Domain.EventsTags;

public class EventTag
{
    public EventTagId EventTagId { get; }
    public EventId EventId { get; }
    public Event? Event { get; }
    public TagId TagId { get;}
    public Tag? Tag { get; }

    private EventTag(EventTagId eventTagId, EventId eventId, TagId tagId)
    {
        EventTagId = eventTagId;
        EventId = eventId;
        TagId = tagId;
    }
    
    public static EventTag New(EventTagId eventTagId, EventId eventId, TagId tagId) =>
    new(eventTagId, eventId, tagId);
}