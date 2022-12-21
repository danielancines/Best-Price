using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Maui.BestPrice.App.Models;

public class Medicine
{
    public string Name { get; set; }

    public float Price { get; set; }

    public string Thumbnail { get; set; }

    public string DrugStore { get; set; }

    public float OldPrice { get; set; }

    public DateTime ExpireDate { get; set; }
}