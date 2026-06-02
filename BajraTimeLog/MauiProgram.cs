using Microsoft.Extensions.Logging;
using BajraTimeLog.Services;
using BajraTimeLog.ViewModels;
using BajraTimeLog.Views;

namespace BajraTimeLog;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",   "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf",  "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IOdooService, OdooService>();

        // ViewModels  (Transient so state is fresh each navigation)
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TimeLogViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TimeLogPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
