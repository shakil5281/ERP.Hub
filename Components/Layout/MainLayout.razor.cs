using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ERPHub.Services;

namespace ERPHub.Components.Layout;

public partial class MainLayout
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private string _globalSearchQuery = string.Empty;
    private bool _showProfileDropdown = false;

    private void ToggleProfileDropdown()
    {
        _showProfileDropdown = !_showProfileDropdown;
    }

    private void CloseProfileDropdown()
    {
        _showProfileDropdown = false;
    }

    private async Task ToggleSidebar()
    {
        await JS.InvokeVoidAsync("sidebarFunctions.toggle");
    }

    private async Task HandleLogout()
    {
        CloseProfileDropdown();
        var customProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
        await customProvider.UpdateAuthenticationState(null);
        NavigationManager.NavigateTo("/login", forceLoad: true);
    }

    private bool _showNotificationsPopover = false;

    private List<NotificationItem> _notifications = new()
    {
        new()
        {
            Title = "OT Approval Needed",
            Message = "3 Overtime applications are pending review.",
            Timestamp = DateTime.Now.AddMinutes(-5),
            IsRead = false,
            Icon = "💰",
            LinkUrl = "ot/daily-sheet"
        },
        new()
        {
            Title = "Production Budget Alert",
            Message = "Overtime costs have reached 92% of the budget threshold.",
            Timestamp = DateTime.Now.AddHours(-2),
            IsRead = false,
            Icon = "🚨",
            LinkUrl = "ot/daily-summary"
        },
        new()
        {
            Title = "Punctuality Alert",
            Message = "Steve Rogers checked in Late today by 25 minutes.",
            Timestamp = DateTime.Now.AddHours(-4),
            IsRead = false,
            Icon = "📅",
            LinkUrl = "attendance/daily"
        }
    };

    private int UnreadCount => _notifications.Count(n => !n.IsRead);

    private void ToggleNotificationsPopover()
    {
        _showNotificationsPopover = !_showNotificationsPopover;
        if (_showNotificationsPopover)
        {
            CloseProfileDropdown();
        }
    }

    private void MarkAllAsRead()
    {
        foreach (var note in _notifications)
        {
            note.IsRead = true;
        }
    }

    private void NavigateToLink(NotificationItem note)
    {
        note.IsRead = true;
        _showNotificationsPopover = false;
        if (!string.IsNullOrEmpty(note.LinkUrl))
        {
            NavigationManager.NavigateTo(note.LinkUrl);
        }
    }

    private string FormatTimeAgo(DateTime timestamp)
    {
        var diff = DateTime.Now - timestamp;
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        return timestamp.ToString("MMM dd, hh:mm tt");
    }
}

public class NotificationItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    public string Icon { get; set; } = "🔔";
    public string LinkUrl { get; set; } = string.Empty;
}

