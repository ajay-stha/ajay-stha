using BajraTimeLog.ViewModels;

namespace BajraTimeLog.Views;

public partial class TimeLogPage : ContentPage
{
    private readonly TimeLogViewModel _viewModel;

    public TimeLogPage(TimeLogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Limit date picker to today
        DatePickerControl.MaximumDate = DateTime.Today;
        // Load tasks (no-op if already loaded)
        _ = _viewModel.InitializeAsync();
    }
}
