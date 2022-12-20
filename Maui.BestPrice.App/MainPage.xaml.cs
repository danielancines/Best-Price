using Maui.BestPrice.App.ViewModels;

namespace Maui.BestPrice.App;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}


