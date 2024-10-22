namespace Domain.Locations;

public class Location
{
    public LocationId Id { get; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string Country  { get; private set; }
    public int Capacity { get; private set; }

    private Location(LocationId id, string name, string address, string city, string country, int capacity)
    {
        Id = id;
        Name = name;
        Address = address;
        City = city;
        Country = country;
        Capacity = capacity;
    }
    
    public static Location New(LocationId courseId, string name, string address, string city, string country, int capacity)
        => new(courseId, name, address, city, country, capacity);
    
    public void UpdateDetails(string name,string address, string city, string country, int capacity )
    {
        Name = name;
        Address = address;
        City = city;
        Country = country;
        Capacity = capacity;
    }
}
