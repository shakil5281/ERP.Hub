using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ERPHub.Services;

namespace ERPHub.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private CustomAuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    private bool _drawerOpen = true;
    private bool _showSearchDialog;
    private bool _showNotifications;
    private bool _showProfileMenu;
    private IReadOnlyList<NotificationItem> _notifications = Array.Empty<NotificationItem>();
    private int _unreadCount;

    protected override void OnInitialized()
    {
        NotificationService.OnNotificationsChanged += OnNotificationsChanged;
        NotificationService.SeedDemoNotifications();
        RefreshNotifications();
    }

    private void OnNotificationsChanged()
    {
        RefreshNotifications();
        InvokeAsync(StateHasChanged);
    }

    private void RefreshNotifications()
    {
        _notifications = NotificationService.Notifications;
        _unreadCount = NotificationService.UnreadCount;
    }

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;

    private void OpenSearchDialog()
    {
        CloseAllPanels();
        _showSearchDialog = true;
    }

    private void CloseSearchDialog()
    {
        _showSearchDialog = false;
    }

    private void ToggleNotifications()
    {
        _showProfileMenu = false;
        _showNotifications = !_showNotifications;
    }

    private void CloseNotifications() => _showNotifications = false;

    private void ToggleProfileMenu()
    {
        _showNotifications = false;
        _showProfileMenu = !_showProfileMenu;
    }

    private void CloseProfileMenu() => _showProfileMenu = false;

    private void CloseAllPanels()
    {
        _showNotifications = false;
        _showProfileMenu = false;
    }

    private void MarkAllRead()
    {
        NotificationService.MarkAllAsRead();
    }

    private void HandleNotificationClick(NotificationItem note)
    {
        NotificationService.MarkAsRead(note.Id);
        _showNotifications = false;
        if (!string.IsNullOrEmpty(note.LinkUrl))
        {
            Nav.NavigateTo(note.LinkUrl);
        }
    }

    private async Task HandleLogout()
    {
        CloseAllPanels();
        await AuthProvider.UpdateAuthenticationState(null);
        Nav.NavigateTo("/login", forceLoad: true);
    }

    private static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][..1].ToUpper();
        return $"{parts[0][..1]}{parts[^1][..1]}".ToUpper();
    }

    private static string GetUserRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
    }

    private static string FormatTimeAgo(DateTime timestamp)
    {
        var diff = DateTime.Now - timestamp;
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        return timestamp.ToString("MMM dd, yyyy");
    }

    public void Dispose()
    {
        NotificationService.OnNotificationsChanged -= OnNotificationsChanged;
    }
}
