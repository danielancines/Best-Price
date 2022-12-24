using System.Globalization;

namespace Maui.BestPrice.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var culture = new CultureInfo("pt-BR");
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        MainPage = new AppShell();
    }
}

