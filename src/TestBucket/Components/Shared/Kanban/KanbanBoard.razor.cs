﻿using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TestBucket.Components.Shared.Kanban;

public partial class KanbanBoard<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
{
    [Parameter] public IEnumerable<T> Items { get; set; } = [];
    [Parameter] public List<string>? Columns { get; set; }

    private List<string> _columns = [];

    [Parameter] public RenderFragment<T>? ItemRenderer { get; set; }

    /// <summary>
    /// The function which groups values in this column.
    /// </summary>
    [Parameter] public Func<T, string>? GetColumnFunc { get; set; }

    [Parameter] public Func<T, int>? ColorHashFunc { get; set; }

    [Parameter] public Func<T, string, ValueTask>? SetColumnFunc { get; set; }

    private IEnumerable<string> GetColumns()
    {
        return _columns;
    }

    private async Task OnDroppedOnColumn(T item, string column)
    {
        if (SetColumnFunc is not null)
        {
            await SetColumnFunc(item, column);
        }
    }

    protected override void OnInitialized()
    {
        if (Columns == null)
        {
            if (GetColumnFunc is not null)
            {
                _columns = Items.Select(x => GetColumnFunc(x)).Distinct().ToList();
            }
        }
        else
        {
            _columns.Clear();
            _columns.AddRange(Columns);
        }
    }

    protected override void OnParametersSet()
    {
        OnInitialized();
    }

    private IEnumerable<T> GetItemsInColumn(string column)
    {
        if (GetColumnFunc is not null)
        {
            foreach (var item in Items)
            {
                string itemColumn = GetColumnFunc(item);
                if(itemColumn == column)
                {
                    yield return item;
                }
            }
        }
    }
}
