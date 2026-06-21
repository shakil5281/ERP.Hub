using System.Collections.Generic;

namespace ERPHub.Models
{
    public class ImportResultDto
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
