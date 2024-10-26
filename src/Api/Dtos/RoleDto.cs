using Domain.Roles;

namespace Api.Dtos;

public record RoleDto(
    Guid? Id,
    string Title)
{
    public static RoleDto FromDomainModel(Role role)
        => new(
            Id: role.Id.Value,
            Title: role.Title);
}