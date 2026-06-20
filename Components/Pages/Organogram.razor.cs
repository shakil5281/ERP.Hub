using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using ERPHub.Data;
using ERPHub.Models;

namespace ERPHub.Components.Pages;

public partial class Organogram
{
    [Inject] private ErpDbContext DbContext { get; set; } = default!;

    private string _activeTab = "department";
    private int _activeTabIndex = 0; // department is default

    private readonly Dictionary<int, string> _tabMap = new()
    {
        { 0, "department" },
        { 1, "section" },
        { 2, "designation" },
        { 3, "line" }
    };

    private void OnTabChanged(int index)
    {
        _activeTabIndex = index;
        _activeTab = _tabMap.TryGetValue(index, out var tab) ? tab : "department";
        StateHasChanged();
    }

    // Department
    private List<Department> _departments = new();
    private string _deptSearch = string.Empty;
    private bool _showDeptModal;
    private bool _isEditDept;
    private Department _editingDept = new();

    // Section
    private List<Section> _sections = new();
    private string _sectionSearch = string.Empty;
    private int _sectionDeptFilter;
    private bool _showSectionModal;
    private bool _isEditSection;
    private Section _editingSection = new();

    // Designation
    private List<Designation> _designations = new();
    private string _desigSearch = string.Empty;
    private int _desigSectionFilter;
    private bool _showDesigModal;
    private bool _isEditDesig;
    private Designation _editingDesig = new();

    // Line
    private List<Line> _lines = new();
    private string _lineSearch = string.Empty;
    private int _lineSectionFilter;
    private bool _showLineModal;
    private bool _isEditLine;
    private Line _editingLine = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadDepartments();
        await LoadSections();
        await LoadDesignations();
        await LoadLines();
    }

    // --- Department ---

    private async Task LoadDepartments()
    {
        _departments = await DbContext.Departments.OrderByDescending(d => d.Id).ToListAsync();
    }

    private IEnumerable<Department> FilteredDepartments => _departments
        .Where(d => string.IsNullOrWhiteSpace(_deptSearch) ||
                    d.NameEn.Contains(_deptSearch, StringComparison.OrdinalIgnoreCase) ||
                    d.NameBn.Contains(_deptSearch));

    private void OpenAddDept()
    {
        _isEditDept = false;
        _editingDept = new Department();
        _showDeptModal = true;
    }

    private void OpenEditDept(Department dept)
    {
        _isEditDept = true;
        _editingDept = new Department { Id = dept.Id, NameEn = dept.NameEn, NameBn = dept.NameBn };
        _showDeptModal = true;
    }

    private void CloseDeptModal() => _showDeptModal = false;

    private async Task SaveDept()
    {
        if (_isEditDept)
            DbContext.Departments.Update(_editingDept);
        else
            DbContext.Departments.Add(_editingDept);

        await DbContext.SaveChangesAsync();
        await LoadDepartments();
        _showDeptModal = false;
    }

    private async Task ConfirmDeleteDept(int id)
    {
        var entity = await DbContext.Departments.FindAsync(id);
        if (entity != null)
        {
            // Manual cascade delete
            var sections = await DbContext.Sections.Where(s => s.DepartmentId == id).ToListAsync();
            foreach (var section in sections)
            {
                var designations = await DbContext.Designations.Where(d => d.SectionId == section.Id).ToListAsync();
                var lines = await DbContext.Lines.Where(l => l.SectionId == section.Id).ToListAsync();
                DbContext.Designations.RemoveRange(designations);
                DbContext.Lines.RemoveRange(lines);
            }
            DbContext.Sections.RemoveRange(sections);

            DbContext.Departments.Remove(entity);
            await DbContext.SaveChangesAsync();
            
            await LoadDepartments();
            await LoadSections();
            await LoadDesignations();
            await LoadLines();
        }
    }

    // --- Section ---

    private async Task LoadSections()
    {
        var raw = await DbContext.Sections.OrderByDescending(s => s.Id).ToListAsync();
        _sections = raw.Select(s => new Section
        {
            Id = s.Id,
            NameEn = s.NameEn,
            NameBn = s.NameBn,
            DepartmentId = s.DepartmentId,
            DepartmentNameEn = _departments.FirstOrDefault(d => d.Id == s.DepartmentId)?.NameEn ?? "Unknown"
        }).ToList();
    }

    private IEnumerable<Section> FilteredSections => _sections
        .Where(s => (string.IsNullOrWhiteSpace(_sectionSearch) ||
                     s.NameEn.Contains(_sectionSearch, StringComparison.OrdinalIgnoreCase) ||
                     s.NameBn.Contains(_sectionSearch)) &&
                    (_sectionDeptFilter == 0 || s.DepartmentId == _sectionDeptFilter));

    private void OpenAddSection()
    {
        _isEditSection = false;
        _editingSection = new Section();
        _showSectionModal = true;
    }

    private void OpenEditSection(Section section)
    {
        _isEditSection = true;
        _editingSection = new Section
        {
            Id = section.Id,
            NameEn = section.NameEn,
            NameBn = section.NameBn,
            DepartmentId = section.DepartmentId
        };
        _showSectionModal = true;
    }

    private void CloseSectionModal() => _showSectionModal = false;

    private async Task SaveSection()
    {
        if (_isEditSection)
        {
            var existing = await DbContext.Sections.FindAsync(_editingSection.Id);
            if (existing != null)
            {
                existing.NameEn = _editingSection.NameEn;
                existing.NameBn = _editingSection.NameBn;
                existing.DepartmentId = _editingSection.DepartmentId;
            }
        }
        else
        {
            DbContext.Sections.Add(_editingSection);
        }
        await DbContext.SaveChangesAsync();
        await LoadSections();
        _showSectionModal = false;
    }

    private async Task ConfirmDeleteSection(int id)
    {
        var entity = await DbContext.Sections.FindAsync(id);
        if (entity != null)
        {
            // Manual cascade delete
            var designations = await DbContext.Designations.Where(d => d.SectionId == id).ToListAsync();
            var lines = await DbContext.Lines.Where(l => l.SectionId == id).ToListAsync();
            DbContext.Designations.RemoveRange(designations);
            DbContext.Lines.RemoveRange(lines);

            DbContext.Sections.Remove(entity);
            await DbContext.SaveChangesAsync();
            
            await LoadSections();
            await LoadDesignations();
            await LoadLines();
        }
    }

    // --- Designation ---

    private async Task LoadDesignations()
    {
        var raw = await DbContext.Designations.OrderByDescending(d => d.Id).ToListAsync();
        _designations = raw.Select(d => new Designation
        {
            Id = d.Id,
            NameEn = d.NameEn,
            NameBn = d.NameBn,
            SectionId = d.SectionId,
            SectionNameEn = _sections.FirstOrDefault(s => s.Id == d.SectionId)?.NameEn ?? "Unknown"
        }).ToList();
    }

    private IEnumerable<Designation> FilteredDesignations => _designations
        .Where(d => (string.IsNullOrWhiteSpace(_desigSearch) ||
                     d.NameEn.Contains(_desigSearch, StringComparison.OrdinalIgnoreCase) ||
                     d.NameBn.Contains(_desigSearch)) &&
                    (_desigSectionFilter == 0 || d.SectionId == _desigSectionFilter));

    private void OpenAddDesig()
    {
        _isEditDesig = false;
        _editingDesig = new Designation();
        _showDesigModal = true;
    }

    private void OpenEditDesig(Designation desig)
    {
        _isEditDesig = true;
        _editingDesig = new Designation
        {
            Id = desig.Id,
            NameEn = desig.NameEn,
            NameBn = desig.NameBn,
            SectionId = desig.SectionId
        };
        _showDesigModal = true;
    }

    private void CloseDesigModal() => _showDesigModal = false;

    private async Task SaveDesig()
    {
        if (_isEditDesig)
        {
            var existing = await DbContext.Designations.FindAsync(_editingDesig.Id);
            if (existing != null)
            {
                existing.NameEn = _editingDesig.NameEn;
                existing.NameBn = _editingDesig.NameBn;
                existing.SectionId = _editingDesig.SectionId;
            }
        }
        else
        {
            DbContext.Designations.Add(_editingDesig);
        }
        await DbContext.SaveChangesAsync();
        await LoadDesignations();
        _showDesigModal = false;
    }

    private async Task ConfirmDeleteDesig(int id)
    {
        var entity = await DbContext.Designations.FindAsync(id);
        if (entity != null)
        {
            DbContext.Designations.Remove(entity);
            await DbContext.SaveChangesAsync();
            await LoadDesignations();
        }
    }

    // --- Line ---

    private async Task LoadLines()
    {
        var raw = await DbContext.Lines.OrderByDescending(l => l.Id).ToListAsync();
        _lines = raw.Select(l => new Line
        {
            Id = l.Id,
            NameEn = l.NameEn,
            NameBn = l.NameBn,
            SectionId = l.SectionId,
            SectionNameEn = _sections.FirstOrDefault(s => s.Id == l.SectionId)?.NameEn ?? "Unknown"
        }).ToList();
    }

    private IEnumerable<Line> FilteredLines => _lines
        .Where(l => (string.IsNullOrWhiteSpace(_lineSearch) ||
                     l.NameEn.Contains(_lineSearch, StringComparison.OrdinalIgnoreCase) ||
                     l.NameBn.Contains(_lineSearch)) &&
                    (_lineSectionFilter == 0 || l.SectionId == _lineSectionFilter));

    private void OpenAddLine()
    {
        _isEditLine = false;
        _editingLine = new Line();
        _showLineModal = true;
    }

    private void OpenEditLine(Line line)
    {
        _isEditLine = true;
        _editingLine = new Line
        {
            Id = line.Id,
            NameEn = line.NameEn,
            NameBn = line.NameBn,
            SectionId = line.SectionId
        };
        _showLineModal = true;
    }

    private void CloseLineModal() => _showLineModal = false;

    private async Task SaveLine()
    {
        if (_isEditLine)
        {
            var existing = await DbContext.Lines.FindAsync(_editingLine.Id);
            if (existing != null)
            {
                existing.NameEn = _editingLine.NameEn;
                existing.NameBn = _editingLine.NameBn;
                existing.SectionId = _editingLine.SectionId;
            }
        }
        else
        {
            DbContext.Lines.Add(_editingLine);
        }
        await DbContext.SaveChangesAsync();
        await LoadLines();
        _showLineModal = false;
    }

    private async Task ConfirmDeleteLine(int id)
    {
        var entity = await DbContext.Lines.FindAsync(id);
        if (entity != null)
        {
            DbContext.Lines.Remove(entity);
            await DbContext.SaveChangesAsync();
            await LoadLines();
        }
    }
}
