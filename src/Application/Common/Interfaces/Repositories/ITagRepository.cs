using Domain.Tags;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ITagRepository
{
    Task<Option<Tag>> GetById(TagId id, CancellationToken cancellationToken);
    Task<Option<Tag>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Tag> Add(Tag tag, CancellationToken cancellationToken);
    Task<Tag> Update(Tag tag, CancellationToken cancellationToken);
    Task<Tag> Delete(Tag tag, CancellationToken cancellationToken);
}