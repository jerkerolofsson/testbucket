using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Labels;
using TestBucket.Domain.Labels.Models;

namespace TestBucket.Domain.UnitTests.Labels;

/// <summary>
/// A fake implementation of the <see cref="ILabelRepository"/> for testing purposes.
/// </summary>
internal class FakeLabelRepository : ILabelRepository
{
    private readonly List<Label> _labels = new();

    public Task AddLabelAsync(Label label)
    {
        _labels.Add(label);
        return Task.CompletedTask;
    }

    public Task DeleteLabelByIdAsync(long id)
    {
        _labels.RemoveAll(label => label.Id == id);
        return Task.CompletedTask;
    }

    public Task<Label?> GetLabelByIdAsync(long id)
    {
        return Task.FromResult(_labels.FirstOrDefault(label => label.Id == id));
    }

    public Task<IReadOnlyList<Label>> GetLabelsAsync(IEnumerable<FilterSpecification<Label>> filters)
    {
        var query = _labels.AsQueryable();
        foreach (var filter in filters)
        {
            query = query.Where(filter.Expression);
        }
        return Task.FromResult<IReadOnlyList<Label>>(query.ToList());
    }

    public Task UpdateLabelAsync(Label label)
    {
        var existingLabel = _labels.FirstOrDefault(l => l.Id == label.Id);
        if (existingLabel != null)
        {
            _labels.Remove(existingLabel);
            _labels.Add(label);
        }
        return Task.CompletedTask;
    }
}
