namespace ERPHub.Models;

public static class PayrollRunStatus
{
    public const string Draft = "Draft";
    public const string Calculated = "Calculated";
    public const string Verified = "Verified";
    public const string Approved = "Approved";
    public const string Locked = "Locked";
}

public static class PayrollApprovalStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Paid = "Paid";
    public const string Closed = "Closed";
    public const string Rejected = "Rejected";
}
