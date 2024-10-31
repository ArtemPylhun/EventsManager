using Application.Categories.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands;

public record CreateCategoryCommand : IRequest<Result<Category, CategoryException>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICategoryQueries categoryQueries) : IRequestHandler<CreateCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var existingCategory = await categoryQueries.SearchByName(request.Name, cancellationToken);

        return await existingCategory.Match(
            c => Task.FromResult<Result<Category, CategoryException>>(new CategoryAlreadyExistsException(c.Id)),
            async () => await CreateEntity(request.Name, request.Description, cancellationToken));
    }

    private async Task<Result<Category, CategoryException>> CreateEntity(
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Category.New(CategoryId.New(), name, description);

            return await categoryRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownException(CategoryId.Empty(), exception);
        }
    }
}