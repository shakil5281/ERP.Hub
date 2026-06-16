using System.Collections.Generic;

namespace ERPHub.Models
{
    public class CompanyNodeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameBn { get; set; } = string.Empty;
        public List<DepartmentNodeDto> Departments { get; set; } = new();
    }

    public class DepartmentNodeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameBn { get; set; } = string.Empty;
        public List<SectionNodeDto> Sections { get; set; } = new();
    }

    public class SectionNodeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameBn { get; set; } = string.Empty;
        public List<DesignationNodeDto> Designations { get; set; } = new();
        public List<LineNodeDto> Lines { get; set; } = new();
    }

    public class DesignationNodeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameBn { get; set; } = string.Empty;
    }

    public class LineNodeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameBn { get; set; } = string.Empty;
    }
}
