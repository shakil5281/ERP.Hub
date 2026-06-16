using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Shift
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager NavManager { get; set; } = default!;

    private List<ERPHub.Models.Shift> _shifts = new();

    private string _searchQuery = string.Empty;

    private bool _showFormModal = false;
    private bool _isEditMode   = false;
    private bool _isSaving     = false;
    private ERPHub.Models.Shift _editingShift = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadShifts();
    }

    private async Task LoadShifts()
    {
        try
        {
            var baseUri = NavManager.BaseUri;
            _shifts = await Http.GetFromJsonAsync<List<ERPHub.Models.Shift>>($"{baseUri}api/shifts") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading shifts: {ex.Message}");
            ShowErrorToast("Failed to load shifts.");
        }
    }

    private IEnumerable<ERPHub.Models.Shift> FilteredShifts => _shifts
        .Where(s => string.IsNullOrWhiteSpace(_searchQuery) ||
                    s.ShiftName.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    s.OffDay.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));

    private void OpenAddModal()
    {
        _isEditMode   = false;
        _editingShift = new ERPHub.Models.Shift();
        _showFormModal = true;
    }

    private void OpenEditModal(ERPHub.Models.Shift shift)
    {
        _isEditMode = true;
        _editingShift = new ERPHub.Models.Shift
        {
            Id        = shift.Id,
            ShiftName = shift.ShiftName,
            InTime    = shift.InTime,
            OutTime   = shift.OutTime,
            LateTime  = shift.LateTime,
            OffDay    = shift.OffDay
        };
        _showFormModal = true;
    }

    private void CloseModal()
    {
        _showFormModal = false;
        _isSaving      = false;
    }

    private async Task SaveShift()
    {
        _isSaving = true;
        try
        {
            var baseUri = NavManager.BaseUri;
            bool success;
            if (_isEditMode)
            {
                var response = await Http.PutAsJsonAsync($"{baseUri}api/shifts/{_editingShift.Id}", _editingShift);
                success = response.IsSuccessStatusCode;
            }
            else
            {
                var response = await Http.PostAsJsonAsync($"{baseUri}api/shifts", _editingShift);
                success = response.IsSuccessStatusCode;
            }

            if (success)
            {
                ShowToast(_isEditMode ? "Shift updated successfully." : "Shift added successfully.");
                await LoadShifts();
                _showFormModal = false;
            }
            else
            {
                ShowErrorToast("Failed to save shift. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving shift: {ex.Message}");
            ShowErrorToast($"Error: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task ConfirmDelete(int id)
    {
        try
        {
            var baseUri = NavManager.BaseUri;
            var response = await Http.DeleteAsync($"{baseUri}api/shifts/{id}");
            if (response.IsSuccessStatusCode)
            {
                ShowToast("Shift deleted successfully.");
                await LoadShifts();
            }
            else
            {
                ShowErrorToast("Failed to delete shift.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting shift: {ex.Message}");
            ShowErrorToast($"Error: {ex.Message}");
        }
    }

    // --- Helpers ---
    private static string FormatTime(TimeSpan ts)
    {
        var dt = DateTime.Today.Add(ts);
        return dt.ToString("hh:mm tt");
    }

    private static string FormatTimeForInput(TimeSpan ts)
        => ts.ToString(@"hh\:mm");

    private static TimeSpan ParseTime(string? value)
    {
        if (TimeSpan.TryParse(value, out var ts)) return ts;
        return TimeSpan.Zero;
    }

    // --- Toast Helpers ---
    private bool _showToast    = false;
    private string _toastMessage = string.Empty;
    private bool _showErrorToast = false;
    private string _errorMessage  = string.Empty;

    private void ShowToast(string message)
    {
        _toastMessage    = message;
        _showToast       = true;
        _showErrorToast  = false;
        StateHasChanged();
        Task.Delay(3000).ContinueWith(_ =>
        {
            _showToast = false;
            InvokeAsync(StateHasChanged);
        });
    }

    private void ShowErrorToast(string message)
    {
        _errorMessage    = message;
        _showErrorToast  = true;
        _showToast       = false;
        StateHasChanged();
        Task.Delay(3000).ContinueWith(_ =>
        {
            _showErrorToast = false;
            InvokeAsync(StateHasChanged);
        });
    }
}
