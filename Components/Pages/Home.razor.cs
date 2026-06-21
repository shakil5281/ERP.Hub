using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using ERPHub.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Home
{
    [Inject] private IErpService ErpService { get; set; } = default!;

    private decimal _totalRevenue;
    private int _totalStock;
    private int _pendingTasks;
    private int _paidInvoices;
    private List<Invoice> _invoices = new();
    private List<ActivityItem> _activities = new();

    protected override async Task OnInitializedAsync()
    {
        _totalRevenue = await ErpService.GetTotalRevenueAsync();
        _totalStock = await ErpService.GetTotalStockAsync();
        _pendingTasks = await ErpService.GetPendingTasksCountAsync();
        _paidInvoices = await ErpService.GetPaidInvoicesCountAsync();
        _invoices = await ErpService.GetInvoicesAsync();

        LoadActivities();
    }

    private void LoadActivities()
    {
        _activities = new List<ActivityItem>
        {
            new("Stark Enterprises database sync in progress", "2 mins ago", "var(--warning)"),
            new("Invoice INV-2026-001 paid by Acme Global", "2 hours ago", "var(--success)"),
            new("New Product added: Logitech MX Master 3S", "1 day ago", "var(--info)"),
            new("Task 'Release Blazor Dashboard v1.0' completed", "2 days ago", "var(--primary)"),
            new("Dunning notices sent to Globex accounts department", "3 days ago", "var(--danger)")
        };
    }

    private MudBlazor.Color GetStatusColor(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Paid => MudBlazor.Color.Success,
        InvoiceStatus.Sent => MudBlazor.Color.Warning,
        _ => MudBlazor.Color.Default
    };

    private record ActivityItem(string Title, string TimeDescription, string Color);
}
