namespace Domain.Locations;

public class Location
{
    public LocationId Id { get; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country  { get; set; }
    public int Capacity { get; set; }

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
