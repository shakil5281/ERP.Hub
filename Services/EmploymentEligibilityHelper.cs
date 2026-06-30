using ERPHub.Models;

namespace ERPHub.Services;

public static class EmploymentEligibility
{
    public static bool IsEligibleForProcessing(Employee emp, DateTime processDate)
    {
        if (emp.JoiningDate.Date > processDate.Date)
            return false;

        if (emp.Status == EmployeeStatuses.Active)
            return true;

        if (emp.Status == EmployeeStatuses.Separation && emp.SeparationDate.HasValue)
            return processDate.Date <= emp.SeparationDate.Value.Date;

        return false;
    }

    public static bool IsCurrentlyActive(Employee emp, DateTime? asOfDate = null)
    {
        var date = (asOfDate ?? DateTime.Today).Date;

        if (emp.Status == EmployeeStatuses.Active)
            return emp.JoiningDate.Date <= date;

        if (emp.Status == EmployeeStatuses.Separation && emp.SeparationDate.HasValue)
            return emp.JoiningDate.Date <= date && emp.SeparationDate.Value.Date >= date;

        return false;
    }

    public static IQueryable<Employee> EligibleOnDate(this IQueryable<Employee> query, DateTime processDate) =>
        query.Where(e =>
            e.JoiningDate.Date <= processDate.Date &&
            (e.Status == EmployeeStatuses.Active ||
             (e.Status == EmployeeStatuses.Separation &&
              e.SeparationDate != null &&
              processDate.Date <= e.SeparationDate.Value.Date)));

    public static IQueryable<Employee> CurrentlyActive(this IQueryable<Employee> query, DateTime? asOfDate = null)
    {
        var date = (asOfDate ?? DateTime.Today).Date;
        return query.Where(e =>
            e.JoiningDate.Date <= date &&
            (e.Status == EmployeeStatuses.Active ||
             (e.Status == EmployeeStatuses.Separation &&
              e.SeparationDate != null &&
              e.SeparationDate.Value.Date >= date)));
    }

    public static IQueryable<Employee> SeparatedOnly(this IQueryable<Employee> query) =>
        query.Where(e => e.Status == EmployeeStatuses.Separation);

    public static string NormalizeImportStatus(string? status, out string? separationType)
    {
        separationType = null;
        if (string.IsNullOrWhiteSpace(status) || status.Equals("Regular", StringComparison.OrdinalIgnoreCase))
            return EmployeeStatuses.Active;

        if (status.Equals(EmployeeStatuses.Active, StringComparison.OrdinalIgnoreCase))
            return EmployeeStatuses.Active;

        if (status.Equals(EmployeeStatuses.Separation, StringComparison.OrdinalIgnoreCase))
            return EmployeeStatuses.Separation;

        if (SeparationTypes.IsValid(status))
        {
            separationType = status;
            return EmployeeStatuses.Separation;
        }

        return EmployeeStatuses.Active;
    }
}
