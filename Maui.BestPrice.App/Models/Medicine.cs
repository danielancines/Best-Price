using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Maui.BestPrice.App.Models;

public class Medicine
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("price")]
    public Price Price { get; set; }

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }

    public string DrugStore { get; set; }
}

public class Price
{
    [JsonPropertyName("value")]
    public float Value { get; set; }
}

