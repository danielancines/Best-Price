using Maui.BestPrice.App.Searchers;
using Maui.BestPrice.App.Searchers.Types;
using Maui.BestPrice.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace Maui.BestPrice.App;

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

		builder.Services.AddSingleton<MainPage>();

		//builder.Services.AddTransient<IMedicineSearcher, PrecoPopularSearcher>();
        builder.Services.AddTransient<IMedicineSearcher, DrogaRaiaSearcher>();
        builder.Services.AddTransient<MainPageViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

