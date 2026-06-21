using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ERPHub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class EditEmployee
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager NavManager { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private Employee? _employee;
    private bool _isSaving;
    private bool _isLoading = true;
    private int _activeTabIndex;

    private bool _showToast;
    private string _toastMessage = string.Empty;
    private bool _showErrorToast;
    private string _errorMessage = string.Empty;

    private List<Department> _departments = [];
    private List<Section> _allSections = [];
    private List<Designation> _allDesignations = [];
    private List<Line> _allLines = [];
    private List<ERPHub.Models.Shift> _shifts = [];
    private List<Company> _companies = [];

    private string _gender = string.Empty;
    private DateTime? _dob;
    private DateTime? _joiningDate = DateTime.Today;
    private string _accountType = string.Empty;
    private string _accountNumber = string.Empty;
    private string _accountPlaceholder = "Select account type first";
    private int _accountMaxLength = 17;
    private bool _accountNumberError;
    private string _accountNumberErrorText = string.Empty;

    private decimal _grossSalary;
    private decimal _houseRent;
    private decimal _foodAllowance;
    private decimal _medicalAllowance;
    private decimal _conveyanceAllowance;
    private decimal _totalSalary;

    // Address fields
    private readonly List<AddressDivision> _divisions = ERPHub.Models.BangladeshAddressData.Divisions;
    private int _presentDivisionId;
    private int _presentDistrictId;
    private int _permanentDivisionId;
    private int _permanentDistrictId;
    private bool _sameAsPresent;

    private List<AddressDistrict> _presentFilteredDistricts => _presentDivisionId == 0 ? [] :
        ERPHub.Models.BangladeshAddressData.GetDistrictsByDivision(_presentDivisionId);
    private List<AddressUpazila> _presentFilteredUpazilas => _presentDistrictId == 0 ? [] :
        ERPHub.Models.BangladeshAddressData.GetUpazilasByDistrict(_presentDistrictId);

    private List<AddressDistrict> _permanentFilteredDistricts => _permanentDivisionId == 0 ? [] :
        ERPHub.Models.BangladeshAddressData.GetDistrictsByDivision(_permanentDivisionId);
    private List<AddressUpazila> _permanentFilteredUpazilas => _permanentDistrictId == 0 ? [] :
        ERPHub.Models.BangladeshAddressData.GetUpazilasByDistrict(_permanentDistrictId);

    private IBrowserFile? _photoFile;
    private IBrowserFile? _signatureFile;
    private string? _photoPreview;
    private string? _signaturePreview;

    private List<Section> _filteredSections => _employee?.DepartmentId == 0 ? [] :
        _allSections.Where(s => s.DepartmentId == _employee!.DepartmentId).ToList();

    private List<Designation> _filteredDesignations => _employee?.SectionId == 0 ? [] :
        _allDesignations.Where(d => d.SectionId == _employee!.SectionId).ToList();

    private List<Line> _filteredLines => _employee?.SectionId == 0 ? [] :
        _allLines.Where(l => l.SectionId == _employee!.SectionId).ToList();

    protected override async Task OnInitializedAsync()
    {
        var baseUri = NavManager.BaseUri;
        await LoadLookupDataAsync(baseUri);
        await LoadEmployeeAsync(baseUri);
    }

    private async Task LoadLookupDataAsync(string baseUri)
    {
        try
        {
            _departments = await Http.GetFromJsonAsync<List<Department>>($"{baseUri}api/departments") ?? [];
            _allSections = await Http.GetFromJsonAsync<List<Section>>($"{baseUri}api/sections") ?? [];
            _allDesignations = await Http.GetFromJsonAsync<List<Designation>>($"{baseUri}api/designations") ?? [];
            _allLines = await Http.GetFromJsonAsync<List<Line>>($"{baseUri}api/lines") ?? [];
            _shifts = await Http.GetFromJsonAsync<List<ERPHub.Models.Shift>>($"{baseUri}api/shifts") ?? [];
            _companies = await Http.GetFromJsonAsync<List<Company>>($"{baseUri}api/companies") ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading lookup data: {ex.Message}");
        }
    }

    private async Task LoadEmployeeAsync(string baseUri)
    {
        try
        {
            _employee = await Http.GetFromJsonAsync<Employee>($"{baseUri}api/employees/{Id}");
            if (_employee != null)
            {
                _gender = _employee.Gender;
                if (DateTime.TryParseExact(_employee.DateOfBirth, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dob))
                    _dob = dob;
                _joiningDate = _employee.JoiningDate;
                _grossSalary = _employee.GrossSalary;
                _photoPreview = string.IsNullOrEmpty(_employee.PhotoBase64) ? null : _employee.PhotoBase64;
                _signaturePreview = string.IsNullOrEmpty(_employee.SignatureBase64) ? null : _employee.SignatureBase64;

                // Set address dropdown states
                _presentDivisionId = _employee.PresentDivisionId;
                _presentDistrictId = _employee.PresentDistrictId;
                _permanentDivisionId = _employee.PermanentDivisionId;
                _permanentDistrictId = _employee.PermanentDistrictId;

                // Calculate salary breakdown
                OnGrossSalaryChanged(_grossSalary);

                // Set account type placeholder (this clears _accountNumber, so set account values after)
                _accountType = _employee.AccountType;
                OnAccountTypeChanged(_accountType);
                _accountNumber = _employee.AccountNumber;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading employee: {ex.Message}");
            ShowErrorToast("Failed to load employee data.");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnGrossSalaryChanged(decimal gross)
    {
        _grossSalary = gross;
        if (_employee == null) return;
        _employee.BasicSalary = Math.Round(gross * 0.536m, 2);
        _houseRent = Math.Round(_employee.BasicSalary * 0.50m, 2);
        _foodAllowance = Math.Round(gross * 0.10m, 2);
        _medicalAllowance = Math.Round(gross * 0.06m, 2);
        _conveyanceAllowance = Math.Round(gross * 0.036m, 2);
        _totalSalary = _employee.BasicSalary + _houseRent + _foodAllowance + _medicalAllowance + _conveyanceAllowance;
    }

    private void OnDepartmentValueChanged(int value)
    {
        if (_employee == null) return;
        _employee.DepartmentId = value;
        _employee.SectionId = 0;
        _employee.DesignationId = 0;
        _employee.LineId = 0;
    }

    private void OnSectionValueChanged(int value)
    {
        if (_employee == null) return;
        _employee.SectionId = value;
        _employee.DesignationId = 0;
        _employee.LineId = 0;
    }

    private void OnJoiningDateChanged(DateTime? date)
    {
        _joiningDate = date;
        if (_employee != null)
            _employee.JoiningDate = date ?? DateTime.Today;
    }

    private void OnDobChanged(DateTime? date) => _dob = date;

    private void OnAccountTypeChanged(string value)
    {
        _accountType = value;
        _accountNumber = string.Empty;
        _accountNumberError = false;
        _accountNumberErrorText = string.Empty;

        if (_accountType == "mCash")
        {
            _accountPlaceholder = "Enter 12 digit mCash number";
            _accountMaxLength = 12;
        }
        else if (_accountType == "Card")
        {
            _accountPlaceholder = "Enter 17 digit card number";
            _accountMaxLength = 17;
        }
        else
        {
            _accountPlaceholder = "Select account type first";
            _accountMaxLength = 17;
        }
    }

    private void OnAccountNumberChanged(string value)
    {
        _accountNumber = value;
        ValidateAccountNumber();
    }

    private void ValidateAccountNumber()
    {
        if (string.IsNullOrEmpty(_accountType) || string.IsNullOrEmpty(_accountNumber))
        {
            _accountNumberError = false;
            _accountNumberErrorText = string.Empty;
            return;
        }

        if (_accountType == "mCash" && _accountNumber.Length != 12)
        {
            _accountNumberError = true;
            _accountNumberErrorText = "mCash number must be exactly 12 digits";
        }
        else if (_accountType == "Card" && _accountNumber.Length != 17)
        {
            _accountNumberError = true;
            _accountNumberErrorText = "Card number must be exactly 17 digits";
        }
        else
        {
            _accountNumberError = false;
            _accountNumberErrorText = string.Empty;
        }
    }

    private void OnPresentDivisionChanged(int value)
    {
        _presentDivisionId = value;
        _presentDistrictId = 0;
        if (_employee != null) _employee.PresentUpazilaId = 0;
    }

    private void OnPresentDistrictChanged(int value)
    {
        _presentDistrictId = value;
        if (_employee != null) _employee.PresentUpazilaId = 0;
    }

    private void OnPermanentDivisionChanged(int value)
    {
        _permanentDivisionId = value;
        _permanentDistrictId = 0;
        if (_employee != null) _employee.PermanentUpazilaId = 0;
    }

    private void OnPermanentDistrictChanged(int value)
    {
        _permanentDistrictId = value;
        if (_employee != null) _employee.PermanentUpazilaId = 0;
    }

    private void OnSameAsPresentChanged(bool value)
    {
        if (_employee == null) return;
        _sameAsPresent = value;
        if (_sameAsPresent)
        {
            _employee.PermanentVillage = _employee.PresentVillage;
            _employee.PermanentPostOffice = _employee.PresentPostOffice;
            _permanentDivisionId = _presentDivisionId;
            _permanentDistrictId = _presentDistrictId;
            _employee.PermanentUpazilaId = _employee.PresentUpazilaId;
            _employee.PermanentPostalCode = _employee.PresentPostalCode;
        }
    }

    private async Task OnPhotoSelected(InputFileChangeEventArgs e)
    {
        _photoFile = e.File;
        _photoPreview = await ConvertToDataUrl(_photoFile, 2 * 1024 * 1024);
    }

    private async Task OnSignatureSelected(InputFileChangeEventArgs e)
    {
        _signatureFile = e.File;
        _signaturePreview = await ConvertToDataUrl(_signatureFile, 1 * 1024 * 1024);
    }

    private async Task<string?> ConvertToDataUrl(IBrowserFile file, long maxSize)
    {
        try
        {
            using var stream = file.OpenReadStream(maxSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            var contentType = file.ContentType;
            return $"data:{contentType};base64,{base64}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            ShowErrorToast("File is too large or unsupported format.");
            return null;
        }
    }

    private async Task UpdateEmployee()
    {
        if (_employee == null) return;
        _isSaving = true;
        try
        {
            _employee.DateOfBirth = _dob?.ToString("dd/MM/yyyy") ?? string.Empty;
            _employee.Gender = _gender;
            _employee.AccountType = _accountType;
            _employee.AccountNumber = _accountNumber;
            _employee.GrossSalary = _grossSalary;
            _employee.PhotoBase64 = _photoPreview ?? string.Empty;
            _employee.SignatureBase64 = _signaturePreview ?? string.Empty;

            _employee.Department = null;
            _employee.Section = null;
            _employee.Designation = null;
            _employee.Line = null;
            _employee.Shift = null;
            _employee.Company = null;

            var baseUri = NavManager.BaseUri;
            var response = await Http.PutAsJsonAsync($"{baseUri}api/employees/{Id}", _employee);

            if (response.IsSuccessStatusCode)
            {
                ShowToast("Employee updated successfully.");
                await Task.Delay(1200);
                NavManager.NavigateTo("/hr/employees");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ShowErrorToast($"Failed to update employee. {errorBody}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating employee: {ex.Message}");
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
