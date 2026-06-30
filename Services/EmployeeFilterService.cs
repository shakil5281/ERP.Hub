using ERPHub.Models;

namespace ERPHub.Services
{
    public class EmployeeFilterCriteria
    {
        public int CompanyIdFilter { get; set; }
        public int DepartmentIdFilter { get; set; }
        public int SectionIdFilter { get; set; }
        public int DesignationIdFilter { get; set; }
        public int LineIdFilter { get; set; }
        public int ShiftIdFilter { get; set; }
        public bool HasAnyFilter => CompanyIdFilter > 0 || DepartmentIdFilter > 0 ||
            SectionIdFilter > 0 || DesignationIdFilter > 0 ||
            LineIdFilter > 0 || ShiftIdFilter > 0;
    }

    public class EmployeeFilterService
    {
        public HashSet<string> GetMatchingEmployeeIds(List<Employee> employees, EmployeeFilterCriteria criteria)
        {
            return employees
                .Where(e =>
                    (criteria.CompanyIdFilter == 0 || e.CompanyId == criteria.CompanyIdFilter) &&
                    (criteria.DepartmentIdFilter == 0 || e.DepartmentId == criteria.DepartmentIdFilter) &&
                    (criteria.SectionIdFilter == 0 || e.SectionId == criteria.SectionIdFilter) &&
                    (criteria.DesignationIdFilter == 0 || e.DesignationId == criteria.DesignationIdFilter) &&
                    (criteria.LineIdFilter == 0 || e.LineId == criteria.LineIdFilter) &&
                    (criteria.ShiftIdFilter == 0 || e.ShiftId == criteria.ShiftIdFilter))
                .Select(e => e.EmployeeId)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public List<Employee> FilterEmployees(List<Employee> employees, EmployeeFilterCriteria criteria)
        {
            return employees
                .Where(e =>
                    (criteria.CompanyIdFilter == 0 || e.CompanyId == criteria.CompanyIdFilter) &&
                    (criteria.DepartmentIdFilter == 0 || e.DepartmentId == criteria.DepartmentIdFilter) &&
                    (criteria.SectionIdFilter == 0 || e.SectionId == criteria.SectionIdFilter) &&
                    (criteria.DesignationIdFilter == 0 || e.DesignationId == criteria.DesignationIdFilter) &&
                    (criteria.LineIdFilter == 0 || e.LineId == criteria.LineIdFilter) &&
                    (criteria.ShiftIdFilter == 0 || e.ShiftId == criteria.ShiftIdFilter))
                .ToList();
        }
    }
}
