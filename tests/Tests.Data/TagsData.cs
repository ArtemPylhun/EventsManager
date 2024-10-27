using Domain.Tags;

namespace Tests.Data;

public class TagsData
{
    public static Tag MainTag => Tag.New(TagId.New(), "Sports");
}