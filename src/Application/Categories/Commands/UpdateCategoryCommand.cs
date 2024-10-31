using Application.Categories.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using MediatR;
using Optional;

namespace Application.Categories.Commands;

public record UpdateCategoryCommand : IRequest<Result<Category, CategoryException>>
{
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICategoryQueries categoryQueries) : IRequestHandler<UpdateCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);
        var category = await categoryQueries.GetById(categoryId, cancellationToken);

        return await category.Match(
            async c =>
            {
                var existingCategory = await CheckDuplicated(categoryId, request.Name, cancellationToken);

                return await existingCategory.Match(
                    ec => Task.FromResult<Result<Category, CategoryException>>(new CategoryAlreadyExistsException(ec.Id)),
                    async () => await UpdateEntity(c, request.Name, request.Description, cancellationToken));
            },
            () => Task.FromResult<Result<Category, CategoryException>>(new CategoryNotFoundException(categoryId)));
    }

    private async Task<Result<Category, CategoryException>> UpdateEntity(
        Category category,
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        try
        {
            category.UpdateDetails(name, description);

            return await categoryRepository.Update(category, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownException(category.Id, exception);
        }
    }

    private async Task<Option<Category>> CheckDuplicated(
        CategoryId categoryId,
        string name,
        CancellationToken cancellationToken)
    {
        var category = await categoryQueries.SearchByName(name, cancellationToken);

        return category.Match(
            c => c.Id == categoryId ? Option.None<Category>() : Option.Some(c),
            Option.None<Category>);
    }
}