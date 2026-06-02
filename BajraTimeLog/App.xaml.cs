using BajraTimeLog.Services;

namespace BajraTimeLog;

public partial class App : Application
{
    private readonly IOdooService _odoo;

    public App(IOdooService odoo)
    {
        InitializeComponent();
        _odoo = odoo;
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();
        // Attempt to restore a previous session on the UI thread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var hasSession = await _odoo.RestoreSessionAsync();
                if (hasSession)
                    await Shell.Current.GoToAsync("//timelog");
                // else: default Shell route (//login) is already active
            }
            catch
            {
                // On any error, stay on the login page
            }
        });
    }
}
