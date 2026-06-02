using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BajraTimeLog.Models;
using BajraTimeLog.Services;

namespace BajraTimeLog.ViewModels;

public partial class TimeLogViewModel : BaseViewModel
{
    private readonly IOdooService _odoo;

    [ObservableProperty] private DateTime  _selectedDate   = DateTime.Today;
    [ObservableProperty] private string    _hours          = string.Empty;
    [ObservableProperty] private string    _description    = string.Empty;
    [ObservableProperty] private OdooTask? _selectedTask;
    [ObservableProperty] private string    _errorMessage   = string.Empty;
    [ObservableProperty] private bool      _hasError;
    [ObservableProperty] private string    _successMessage = string.Empty;
    [ObservableProperty] private bool      _hasSuccess;
    [ObservableProperty] private string    _welcomeMessage = string.Empty;
    [ObservableProperty] private bool      _tasksLoaded;

    public ObservableCollection<OdooTask> Tasks { get; } = new();

    public TimeLogViewModel(IOdooService odoo)
    {
        _odoo = odoo;
        Title = "Add Time Log";
    }

    /// <summary>
    /// Called from the page's OnAppearing.  Loads tasks once per session.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (IsBusy || TasksLoaded) return;
        IsBusy = true;
        try
        {
            var userName = await _odoo.GetUserNameAsync();
            WelcomeMessage = $"Hello, {userName ?? "User"} 👋";

            var list = await _odoo.GetMyTasksAsync();
            Tasks.Clear();
            foreach (var t in list)
                Tasks.Add(t);

            TasksLoaded = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (IsBusy) return;

        HasError     = false;
        HasSuccess   = false;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        // Validation
        if (SelectedTask is null)
        {
            ErrorMessage = "Please select a task.";
            HasError = true;
            return;
        }

        if (!double.TryParse(Hours, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var hoursValue)
            || hoursValue <= 0)
        {
            ErrorMessage = "Please enter a valid number of hours (e.g. 2.5).";
            HasError = true;
            return;
        }

        if (hoursValue > 24)
        {
            ErrorMessage = "Hours cannot exceed 24 per day.";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            ErrorMessage = "Please describe the work done.";
            HasError = true;
            return;
        }

        IsBusy = true;
        try
        {
            var entry = new TimeLogEntry
            {
                Date        = SelectedDate.ToString("yyyy-MM-dd"),
                Description = Description.Trim(),
                Hours       = hoursValue,
                TaskId      = SelectedTask.Id,
                ProjectId   = SelectedTask.ProjectId
            };

            var (success, error) = await _odoo.SubmitTimeLogAsync(entry);
            if (success)
            {
                SuccessMessage = $"✅  {hoursValue}h logged on {SelectedDate:MMM d, yyyy}";
                HasSuccess     = true;

                // Reset form
                Hours        = string.Empty;
                Description  = string.Empty;
                SelectedTask = null;
                SelectedDate = DateTime.Today;
            }
            else
            {
                ErrorMessage = error ?? "Failed to submit time log. Please try again.";
                HasError     = true;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _odoo.LogoutAsync();
        TasksLoaded = false;
        Tasks.Clear();
        await Shell.Current.GoToAsync("//login");
    }
}
