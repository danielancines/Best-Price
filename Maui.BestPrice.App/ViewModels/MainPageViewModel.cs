using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.BestPrice.App.Helpers;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    readonly List<IMedicineSearcher> MedicineSearchers = new();
    public OrderedObservableCollection<Medicine> Medicines { get; set; } = new("Price", OrderedCollectionType.Float);
    public string SearchTerm { get; set; }

    public MainPageViewModel()
    {
        
        var searchers = DependencyContainerHelper.GetServices<IMedicineSearcher>();
        this.MedicineSearchers.AddRange(searchers);
    }

    public string FooterAppInfo { get; } = $"{AppInfo.Current.Name} - {AppInfo.Version}";

    [RelayCommand]
    private void Search()
    {
        this.Medicines.Clear();

        foreach (var searcher in this.MedicineSearchers)
            this.Search(searcher);
    }

    private async void Search(IMedicineSearcher searcher)
    {
        var medicines = await searcher.SearchAsync(this.SearchTerm);
        foreach (var medicine in medicines)
            this.Medicines.AddOrdered(medicine);
    }
}

public enum OrderedCollectionType
{
    Float
}

public class OrderedObservableCollection<T> : ObservableCollection<T>
{
    public OrderedObservableCollection(string orderBy, OrderedCollectionType propertyType) : base()
    {
        this.OrderBy = orderBy;
        this.PropertyType = propertyType;
    }
    public string OrderBy { get; init; }

    public OrderedCollectionType PropertyType { get; init; }

    public int AddOrdered(T item)
    {
        if (string.IsNullOrEmpty(this.OrderBy))
            return -1;

        var index = this.GetNextItemIndex(item);
        this.Insert(index, item);

        return index;
    }

    private int GetNextItemIndex(T newItem)
    {
        switch (this.PropertyType)
        {
            case OrderedCollectionType.Float:
                return this.ProcessFloat(newItem);
            default:
                return this.Items.Count;
        }
    }

    private int ProcessFloat(T newItem)
    {
        foreach (var item in this.Items)
            if (this.ParseValueToFloat(item) > this.ParseValueToFloat(newItem))
                return this.Items.IndexOf(item);

        return this.Items.Count;
    }

    private float ParseValueToFloat(T item)
    {
        try
        {
            var property = item.GetType().GetProperty(this.OrderBy);
            return (float)property.GetValue(item);
        }
        catch
        {
            return 0;
        }
    }
}