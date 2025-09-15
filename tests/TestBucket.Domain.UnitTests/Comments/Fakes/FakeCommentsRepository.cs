using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestBucket.Domain.Comments;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Domain.UnitTests.Comments.Fakes;

internal class FakeCommentsRepository : ICommentRepository
{
    private readonly List<Comment> _comments = new();

    public Task AddAsync(Comment comment)
    {
        if (comment == null) throw new ArgumentNullException(nameof(comment));
        // Simulate auto-increment Id if not set
        if (comment.Id == 0)
            comment.Id = _comments.Count > 0 ? _comments.Max(c => c.Id) + 1 : 1;
        _comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comment comment)
    {
        if (comment == null) throw new ArgumentNullException(nameof(comment));
        var toRemove = _comments.FirstOrDefault(c => c.Id == comment.Id);
        if (toRemove != null)
            _comments.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Comment comment)
    {
        if (comment == null) throw new ArgumentNullException(nameof(comment));
        var idx = _comments.FindIndex(c => c.Id == comment.Id);
        if (idx >= 0)
            _comments[idx] = comment;
        return Task.CompletedTask;
    }

    // Optional: For test access
    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();
}