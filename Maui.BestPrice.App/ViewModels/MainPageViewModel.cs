using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.BestPrice.App.Helpers;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	readonly List<IMedicineSearcher> MedicineSearchers = new();

	public ObservableCollection<Medicine> Medicines { get; set; } = new();

	public string SearchTerm { get; set; }

	public MainPageViewModel()
	{
		var searchers = DependencyContainerHelper.GetServices<IMedicineSearcher>();
		this.MedicineSearchers.AddRange(searchers);
	}

	[RelayCommand]
	private void Search()
	{
		this.Medicines.Clear();
		var tasks = new List<Task>();

		foreach (var searcher in this.MedicineSearchers)
		{
			tasks.Add(Task.Run(() =>
			{
                this.Search(searcher);
            }));
		}

		Task.WaitAll(tasks.ToArray());
	}

	private async void Search(IMedicineSearcher searcher)
	{
        var medicines = await (searcher as IMedicineSearcher).SearchAsync(this.SearchTerm);
        foreach (var medicine in medicines)
            this.Medicines.Add(medicine);
    }
}

