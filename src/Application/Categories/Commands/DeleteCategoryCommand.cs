using Application.Categories.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands;

public record DeleteCategoryCommand : IRequest<Result<Category, CategoryException>>
{
    public required Guid CategoryId { get; init; }
}

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICategoryQueries categoryQueries,
    IEventQueries eventQueries) : IRequestHandler<DeleteCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);
        var category = await categoryQueries.GetById(categoryId, cancellationToken);

        return await category.Match(
            async c =>
            {
                var categoryEvents = await eventQueries.GetByCategory(categoryId, cancellationToken);
                if (categoryEvents.Any())
                {
                    return new CategoryHasEventsException(categoryId);
                }
                return await DeleteEntity(c, cancellationToken);
            },
            () => Task.FromResult<Result<Category, CategoryException>>(new CategoryNotFoundException(categoryId)));
    }

    private async Task<Result<Category, CategoryException>> DeleteEntity(Category category,
        CancellationToken cancellationToken)
    {
        try
        {
            return await categoryRepository.Delete(category, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownException(category.Id, exception);
        }
    }
}