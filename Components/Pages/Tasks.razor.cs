using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using ERPHub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Tasks
{
    [Inject] private IErpService ErpService { get; set; } = default!;

    private List<ProjectTask> _tasks = new();
    private bool _showFormModal = false;
    private bool _isEditMode = false;
    private ProjectTask _editingTask = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadTasks();
    }

    private async Task LoadTasks()
    {
        _tasks = await ErpService.GetTasksAsync();
    }

    private string GetStatusLabel(Models.TaskStatus status) => status switch
    {
        Models.TaskStatus.Todo => "To Do",
        Models.TaskStatus.InProgress => "In Progress",
        Models.TaskStatus.Review => "Review Feed",
        Models.TaskStatus.Done => "Completed",
        _ => status.ToString()
    };

    private string GetStatusColor(Models.TaskStatus status) => status switch
    {
        Models.TaskStatus.Todo => "var(--text-muted)",
        Models.TaskStatus.InProgress => "var(--info)",
        Models.TaskStatus.Review => "var(--warning)",
        Models.TaskStatus.Done => "var(--success)",
        _ => "white"
    };

    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "??";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
        return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
    }

    private async Task MoveTask(int taskId, int direction)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            var nextStatusVal = (int)task.Status + direction;
            if (Enum.IsDefined(typeof(Models.TaskStatus), nextStatusVal))
            {
                var newStatus = (Models.TaskStatus)nextStatusVal;
                await ErpService.UpdateTaskStatusAsync(taskId, newStatus);
                await LoadTasks();
            }
        }
    }

    private void OpenAddTaskModal()
    {
        _isEditMode = false;
        _editingTask = new ProjectTask
        {
            Status = Models.TaskStatus.Todo,
            Priority = TaskPriority.Medium,
            DueDate = DateTime.Now.AddDays(7),
            Assignee = "Shakil Ahmed"
        };
        _showFormModal = true;
    }

    private void OpenEditTaskModal(ProjectTask task)
    {
        _isEditMode = true;
        // Deep clone task to edit in isolation
        _editingTask = new ProjectTask
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Assignee = task.Assignee,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
        _showFormModal = true;
    }

    private void CloseModal()
    {
        _showFormModal = false;
    }

    private async Task SaveTask()
    {
        if (_isEditMode)
        {
            await ErpService.UpdateTaskAsync(_editingTask);
        }
        else
        {
            await ErpService.AddTaskAsync(_editingTask);
        }
        
        await LoadTasks();
        _showFormModal = false;
    }

    private async Task DeleteTask(int id)
    {
        await ErpService.DeleteTaskAsync(id);
        await LoadTasks();
    }
}
