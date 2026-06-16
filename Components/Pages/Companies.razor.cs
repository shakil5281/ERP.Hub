using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Companies
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager NavManager { get; set; } = default!;

    private List<Company> _companies = new();

    private string _searchQuery = string.Empty;

    private bool _showFormModal = false;
    private bool _isEditMode = false;
    private Company _editingCompany = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCompanies();
    }

    private async Task LoadCompanies()
    {
        try
        {
            var baseUri = NavManager.BaseUri;
            _companies = await Http.GetFromJsonAsync<List<Company>>($"{baseUri}api/companies") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading companies: {ex.Message}");
        }
    }

    private IEnumerable<Company> FilteredCompanies => _companies
        .Where(c => string.IsNullOrWhiteSpace(_searchQuery) ||
                    c.CompanyNameEn.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.CompanyNameBn.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.AddressEn.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.AddressBn.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    c.PhoneNumber.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));

    private void OpenAddModal()
    {
        _isEditMode = false;
        _editingCompany = new Company();
        _showFormModal = true;
    }

    private void OpenEditModal(Company company)
    {
        _isEditMode = true;
        _editingCompany = new Company
        {
            Id = company.Id,
            CompanyNameEn = company.CompanyNameEn,
            CompanyNameBn = company.CompanyNameBn,
            AddressEn = company.AddressEn,
            AddressBn = company.AddressBn,
            Email = company.Email,
            PhoneNumber = company.PhoneNumber,
            Signature = company.Signature
        };
        _showFormModal = true;
    }

    private void CloseModal()
    {
        _showFormModal = false;
    }

    private async Task SaveCompany()
    {
        try
        {
            var baseUri = NavManager.BaseUri;
            if (_isEditMode)
            {
                await Http.PutAsJsonAsync($"{baseUri}api/companies/{_editingCompany.Id}", _editingCompany);
            }
            else
            {
                await Http.PostAsJsonAsync($"{baseUri}api/companies", _editingCompany);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving company: {ex.Message}");
        }

        await LoadCompanies();
        _showFormModal = false;
    }

    private async Task ConfirmDelete(int id)
    {
        try
        {
            var baseUri = NavManager.BaseUri;
            await Http.DeleteAsync($"{baseUri}api/companies/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting company: {ex.Message}");
        }

        await LoadCompanies();
    }
}
