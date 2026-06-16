using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public enum TaskStatus
    {
        Todo,
        InProgress,
        Review,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class ProjectTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Title must be under 100 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required(ErrorMessage = "Assignee is required")]
        public string Assignee { get; set; } = string.Empty;

        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
