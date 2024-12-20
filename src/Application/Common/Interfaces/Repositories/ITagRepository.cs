﻿using Domain.Tags;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ITagRepository
{
    Task<Tag> Add(Tag tag, CancellationToken cancellationToken);
    Task<Tag> Update(Tag tag, CancellationToken cancellationToken);
    Task<Tag> Delete(Tag tag, CancellationToken cancellationToken);
}