using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class AddEmployee
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager NavManager { get; set; } = default!;

    private Employee _employee = new();
    private bool _isSaving = false;

    private bool _showToast = false;
    private string _toastMessage = string.Empty;
    private bool _showErrorToast = false;
    private string _errorMessage = string.Empty;

    private List<Department> _departments = new();
    private List<Section> _allSections = new();
    private List<Designation> _allDesignations = new();
    private List<Line> _allLines = new();
    private List<ERPHub.Models.Shift> _shifts = new();

    private List<Section> _filteredSections => _employee.DepartmentId == 0 ? new() :
        _allSections.Where(s => s.DepartmentId == _employee.DepartmentId).ToList();

    private List<Designation> _filteredDesignations => _employee.SectionId == 0 ? new() :
        _allDesignations.Where(d => d.SectionId == _employee.SectionId).ToList();

    private List<Line> _filteredLines => _employee.SectionId == 0 ? new() :
        _allLines.Where(l => l.SectionId == _employee.SectionId).ToList();

    protected override async Task OnInitializedAsync()
    {
        var baseUri = NavManager.BaseUri;
        await Task.WhenAll(
            LoadDataAsync(baseUri)
        );
    }

    private async Task LoadDataAsync(string baseUri)
    {
        try
        {
            _departments = await Http.GetFromJsonAsync<List<Department>>($"{baseUri}api/departments") ?? new();
            _allSections = await Http.GetFromJsonAsync<List<Section>>($"{baseUri}api/sections") ?? new();
            _allDesignations = await Http.GetFromJsonAsync<List<Designation>>($"{baseUri}api/designations") ?? new();
            _allLines = await Http.GetFromJsonAsync<List<Line>>($"{baseUri}api/lines") ?? new();
            _shifts = await Http.GetFromJsonAsync<List<ERPHub.Models.Shift>>($"{baseUri}api/shifts") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading lookup data: {ex.Message}");
        }
    }

    private Task OnDepartmentChanged()
    {
        _employee.SectionId = 0;
        _employee.DesignationId = 0;
        _employee.LineId = 0;
        return Task.CompletedTask;
    }

    private Task OnSectionChanged()
    {
        _employee.DesignationId = 0;
        _employee.LineId = 0;
        return Task.CompletedTask;
    }

    private async Task SaveEmployee()
    {
        _isSaving = true;
        try
        {
            var baseUri = NavManager.BaseUri;
            var response = await Http.PostAsJsonAsync($"{baseUri}api/employees", _employee);

            if (response.IsSuccessStatusCode)
            {
                ShowToast("Employee added successfully.");
                await Task.Delay(1200);
                NavManager.NavigateTo("/hr/employees");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ShowErrorToast($"Failed to save employee. {errorBody}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving employee: {ex.Message}");
            ShowErrorToast($"Error: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private void ShowToast(string message)
    {
        _toastMessage = message;
        _showToast = true;
        _showErrorToast = false;
        StateHasChanged();

        Task.Delay(3000).ContinueWith(_ =>
        {
            _showToast = false;
            InvokeAsync(StateHasChanged);
        });
    }

    private void ShowErrorToast(string message)
    {
        _errorMessage = message;
        _showErrorToast = true;
        _showToast = false;
        StateHasChanged();

        Task.Delay(3000).ContinueWith(_ =>
        {
            _showErrorToast = false;
            InvokeAsync(StateHasChanged);
        });
    }
}
