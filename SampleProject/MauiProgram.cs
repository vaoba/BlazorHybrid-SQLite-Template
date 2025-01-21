using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SampleProject.Data;

namespace SampleProject;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        // Register the DbContext as a service so we can inject it into our razor view.
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={DbInitializer.InitializeDbPath()}"));

        return builder.Build();
    }
}