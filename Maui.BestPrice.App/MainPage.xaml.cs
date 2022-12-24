namespace Maui.BestPrice.App;

public partial class MainPage : ContentPage
{
	public MainPage(ViewModels.MainPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}
