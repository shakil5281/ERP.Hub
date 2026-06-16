using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using ERPHub.Data;
using ERPHub.Models;
using ERPHub.Services;
using System.Security.Claims;

namespace ERPHub.Components.Pages;

public partial class Profile
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private ErpDbContext DbContext { get; set; } = default!;
    [Inject] private IErpService ErpService { get; set; } = default!;

    private User? _profileUser;
    private string _editFullName = string.Empty;
    private string _editEmail = string.Empty;

    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _passwordMessage = string.Empty;
    private bool _passwordSuccess;

    private int _totalCompanies;
    private int _totalProducts;
    private int _totalInvoices;

    private bool _showToast;
    private string _toastMessage = string.Empty;

    private List<ActivityItem> _activities = new();

    private bool _hasChanges => _editFullName != (_profileUser?.FullName ?? string.Empty);

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUser();
        await LoadStats();
        LoadActivities();
    }

    private async Task LoadCurrentUser()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            _profileUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (_profileUser != null)
            {
                _editFullName = _profileUser.FullName;
            }
        }
    }

    private async Task LoadStats()
    {
        _totalCompanies = (await ErpService.GetCompaniesAsync()).Count;
        _totalProducts = (await ErpService.GetProductsAsync()).Count;
        _totalInvoices = (await ErpService.GetInvoicesAsync()).Count;
    }

    private void LoadActivities()
    {
        _activities = new List<ActivityItem>
        {
            new("Profile information updated", "2 days ago", "#10b981"),
            new("Password changed successfully", "2 weeks ago", "#6366f1"),
            new("Account created — welcome to ERPHub", "1 month ago", "#f59e0b")
        };
    }

    private async Task SaveProfile()
    {
        if (_profileUser == null) return;

        _profileUser.FullName = _editFullName;
        DbContext.Users.Update(_profileUser);
        await DbContext.SaveChangesAsync();

        var customProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
        await customProvider.UpdateAuthenticationState(new UserSession
        {
            Username = _profileUser.Username,
            FullName = _profileUser.FullName,
            Role = _profileUser.Role
        });

        ShowToast("Profile updated successfully.");
    }

    private async Task ChangePassword()
    {
        _passwordMessage = string.Empty;
        _passwordSuccess = false;

        if (_profileUser == null) return;

        if (string.IsNullOrWhiteSpace(_currentPassword) || string.IsNullOrWhiteSpace(_newPassword) || string.IsNullOrWhiteSpace(_confirmPassword))
        {
            _passwordMessage = "All password fields are required.";
            return;
        }

        if (_newPassword != _confirmPassword)
        {
            _passwordMessage = "New passwords do not match.";
            return;
        }

        if (_newPassword.Length < 6)
        {
            _passwordMessage = "New password must be at least 6 characters.";
            return;
        }

        var currentHash = HashPassword(_currentPassword);
        if (_profileUser.PasswordHash != currentHash)
        {
            _passwordMessage = "Current password is incorrect.";
            return;
        }

        _profileUser.PasswordHash = HashPassword(_newPassword);
        DbContext.Users.Update(_profileUser);
        await DbContext.SaveChangesAsync();

        _passwordMessage = "Password changed successfully.";
        _passwordSuccess = true;
        _currentPassword = string.Empty;
        _newPassword = string.Empty;
        _confirmPassword = string.Empty;

        ShowToast("Password updated successfully.");
    }

    private void ResetForm()
    {
        if (_profileUser != null)
        {
            _editFullName = _profileUser.FullName;
        }
        _editEmail = string.Empty;
    }

    private void ShowToast(string message)
    {
        _toastMessage = message;
        _showToast = true;
    }

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "?";

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][..1].ToUpper();

        return $"{parts[0][..1]}{parts[^1][..1]}".ToUpper();
    }

    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    private record ActivityItem(string Title, string TimeDescription, string Color);
}
