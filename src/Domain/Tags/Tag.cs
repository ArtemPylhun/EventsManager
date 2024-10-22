namespace Domain.Tags;

public class Tag
{
    public TagId Id { get;}
    
    public string Title { get; set; }

    private Tag(TagId id, string title)
    {
        Id = id;
        Title = title;
    }
    
    public static Tag New(TagId courseId, string title)
        => new(courseId, title);
    
    public void UpdateDetails(string title)
    {
        Title = title;
    }
    
}