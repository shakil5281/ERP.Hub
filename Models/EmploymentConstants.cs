namespace ERPHub.Models;

public static class EmployeeStatuses
{
    public const string Active = "Active";
    public const string Separation = "Separation";
}

public static class SeparationTypes
{
    public const string Resign = "Resign";
    public const string Left = "Left";
    public const string Close = "Close";

    public static readonly string[] All = [Resign, Left, Close];

    public static bool IsValid(string? type) =>
        !string.IsNullOrEmpty(type) && All.Contains(type, StringComparer.OrdinalIgnoreCase);
}

public static class SeparationWorkflowStatus
{
    public const string Recorded = "Recorded";
    public const string Settled = "Settled";
    public const string Cancelled = "Cancelled";
}
