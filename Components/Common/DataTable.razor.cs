using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPHub.Components.Common;

public partial class DataTable<TItem> : ComponentBase
{
    [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter] public RenderFragment? TabsContent { get; set; }
    [Parameter] public RenderFragment? FilterContent { get; set; }
    [Parameter] public RenderFragment? ActionsContent { get; set; }
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }
    [Parameter] public RenderFragment<TItem> RowTemplate { get; set; } = default!;
    [Parameter] public RenderFragment<TItem>? RowActionsContent { get; set; }
    [Parameter] public EventCallback<List<TItem>> OnSelectionChanged { get; set; }
    
    private HashSet<TItem> SelectedItems { get; set; } = new();
    private TItem? _activeActionsItem;
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    
    private int TotalCount => Items.Count();
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize) is var pages && pages > 0 ? pages : 1;
    
    private bool CanGoPrev => CurrentPage > 1;
    private bool CanGoNext => CurrentPage < TotalPages;
    
    private IEnumerable<TItem> PaginatedItems => Items
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize);
        
    private bool IsAllSelected => PaginatedItems.Any() && PaginatedItems.All(i => SelectedItems.Contains(i));
    
    protected override void OnParametersSet()
    {
        // Clamp current page if items list updates
        if (CurrentPage > TotalPages)
        {
            CurrentPage = TotalPages;
        }
    }
    
    private void ToggleSelectItem(TItem item)
    {
        if (SelectedItems.Contains(item))
        {
            SelectedItems.Remove(item);
        }
        else
        {
            SelectedItems.Add(item);
        }
        OnSelectionChanged.InvokeAsync(SelectedItems.ToList());
    }
    
    private void ToggleSelectAll(ChangeEventArgs e)
    {
        var isChecked = (bool)(e.Value ?? false);
        if (isChecked)
        {
            foreach (var item in PaginatedItems)
            {
                SelectedItems.Add(item);
            }
        }
        else
        {
            foreach (var item in PaginatedItems)
            {
                SelectedItems.Remove(item);
            }
        }
        OnSelectionChanged.InvokeAsync(SelectedItems.ToList());
    }
    
    private void ToggleRowActions(TItem item)
    {
        if (_activeActionsItem != null && _activeActionsItem.Equals(item))
        {
            _activeActionsItem = default;
        }
        else
        {
            _activeActionsItem = item;
        }
    }
    
    private void OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size))
        {
            PageSize = size;
            CurrentPage = 1;
        }
    }
    
    private void PrevPage()
    {
        if (CanGoPrev) { CurrentPage--; ClosePopover(); }
    }
    
    private void NextPage()
    {
        if (CanGoNext) { CurrentPage++; ClosePopover(); }
    }
    
    private void GoToFirstPage()
    {
        CurrentPage = 1;
        ClosePopover();
    }
    
    private void GoToLastPage()
    {
        CurrentPage = TotalPages;
        ClosePopover();
    }
    
    private void ClosePopover()
    {
        _activeActionsItem = default;
    }
    
    public void ClearSelection()
    {
        SelectedItems.Clear();
        StateHasChanged();
    }
}
