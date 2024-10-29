using Domain.Events;
using Domain.EventsTags;
using Domain.Tags;

namespace Tests.Data;

public class EventTagsData
{
    public static EventTag New(EventId eventId, TagId tagId)
        => EventTag.New(EventTagId.New(), eventId, tagId);
}