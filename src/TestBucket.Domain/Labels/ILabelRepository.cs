using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Labels;
public interface ILabelRepository
{
    Task<Label?> GetLabelByIdAsync(long id);
    Task DeleteLabelByIdAsync(long id);
    Task AddLabelAsync(Label Label);
    Task UpdateLabelAsync(Label Label);
    Task<IReadOnlyList<Label>> GetLabelsAsync(IEnumerable<FilterSpecification<Label>> filters);
}
