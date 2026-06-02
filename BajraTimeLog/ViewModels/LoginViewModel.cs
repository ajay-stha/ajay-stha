using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BajraTimeLog.Services;

namespace BajraTimeLog.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IOdooService _odoo;

    [ObservableProperty] private string _email        = string.Empty;
    [ObservableProperty] private string _password     = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool   _hasError;

    public LoginViewModel(IOdooService odoo)
    {
        _odoo = odoo;
        Title = "Sign In";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        HasError     = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Please enter your email address.";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your password.";
            HasError = true;
            return;
        }

        IsBusy = true;
        try
        {
            var (success, error) = await _odoo.LoginAsync(Email.Trim(), Password);
            if (success)
            {
                Password = string.Empty;
                await Shell.Current.GoToAsync("//timelog");
            }
            else
            {
                ErrorMessage = error ?? "Login failed. Please try again.";
                HasError     = true;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
