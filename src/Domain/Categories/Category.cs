using System.Diagnostics;

namespace Domain.Categories;

public class Category
{
    public CategoryId Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Category(CategoryId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    
    public static Category New(CategoryId id, string name, string description)
        => new(id, name, description);

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}