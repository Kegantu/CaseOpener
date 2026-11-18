using CaseOpener.Services;
using CaseOpener.ViewModels;
using CaseOpener.Views;
using Microsoft.Extensions.Logging;

namespace CaseOpener;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IWalletService, WalletService>();
        builder.Services.AddSingleton<ICaseCatalog, CaseCatalog>();
        builder.Services.AddSingleton<IDropService, DropService>();
        builder.Services.AddSingleton<IAppStateService, AppStateService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<IDeepLinkService, DeepLinkService>();
        builder.Services.AddSingleton<IPricingService, PricingService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IInventoryService, InventoryService>();

        // MVVM
        builder.Services.AddSingleton<StoreViewModel>();
        builder.Services.AddSingleton<StorePage>();
        builder.Services.AddSingleton<InventoryViewModel>();
        builder.Services.AddSingleton<InventoryPage>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();

        builder.Logging.AddDebug();
#if DEBUG
        
#endif
        return builder.Build();
    }
}
