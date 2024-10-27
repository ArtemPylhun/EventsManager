using Domain.Categories;

namespace Api.Dtos;

public record CategoryDto(
    Guid? Id,
    string Name,
    string Description)
{
    public static CategoryDto FromDomainModel(Category category)
     => new(category.Id.Value, category.Name, category.Description);
}