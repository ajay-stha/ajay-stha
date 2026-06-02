using BajraTimeLog.ViewModels;

namespace BajraTimeLog.Views;

public partial class TimeLogPage : ContentPage
{
    public TimeLogPage(TimeLogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DatePickerControl.MaximumDate = DateTime.Today;
        _ = ((TimeLogViewModel)BindingContext).InitializeAsync();
    }
}
