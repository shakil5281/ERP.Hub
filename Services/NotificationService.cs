using System.Collections.Concurrent;

namespace ERPHub.Services;

public class NotificationService
{
    private readonly ConcurrentBag<NotificationItem> _notifications = new();
    private int _unreadCount;

    public event Action? OnNotificationsChanged;

    public IReadOnlyList<NotificationItem> Notifications => _notifications.OrderByDescending(n => n.Timestamp).ToList();
    public int UnreadCount => _unreadCount;

    public void AddNotification(string title, string message, string icon = "🔔", string? linkUrl = null)
    {
        var item = new NotificationItem
        {
            Title = title,
            Message = message,
            Icon = icon,
            LinkUrl = linkUrl ?? string.Empty,
            Timestamp = DateTime.Now,
            IsRead = false
        };
        _notifications.Add(item);
        Interlocked.Increment(ref _unreadCount);
        OnNotificationsChanged?.Invoke();
    }

    public void MarkAsRead(string id)
    {
        var item = _notifications.FirstOrDefault(n => n.Id == id);
        if (item != null && !item.IsRead)
        {
            item.IsRead = true;
            Interlocked.Decrement(ref _unreadCount);
            OnNotificationsChanged?.Invoke();
        }
    }

    public void MarkAllAsRead()
    {
        foreach (var item in _notifications.Where(n => !n.IsRead))
        {
            item.IsRead = true;
        }
        Interlocked.Exchange(ref _unreadCount, 0);
        OnNotificationsChanged?.Invoke();
    }

    public void ClearAll()
    {
        while (_notifications.TryTake(out _)) { }
        Interlocked.Exchange(ref _unreadCount, 0);
        OnNotificationsChanged?.Invoke();
    }

    public void SeedDemoNotifications()
    {
        if (_notifications.Any()) return;

        AddNotification("OT Approval Needed", "3 Overtime applications are pending review.", "💰", "ot/daily-sheet");
        AddNotification("Production Budget Alert", "Overtime costs have reached 92% of the budget threshold.", "🚨", "ot/daily-summary");
        AddNotification("Punctuality Alert", "Steve Rogers checked in Late today by 25 minutes.", "📅", "attendance/daily");
        AddNotification("New Employee Onboarded", "Tony Stark has been added to the HR system.", "👤", "hr/employees");
        AddNotification("Salary Process Complete", "March 2026 payroll has been processed successfully.", "✅", "payroll/salary-sheet");
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
