using System;
using System.Text.Json;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class PanvelSearcher : IMedicineSearcher
{
	readonly string URL = "https://app.grupodimedservices.com.br/prod/app-bff-panvel/v1/search/autocomplete?searchTerm={0}";


    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("app-token", "3JWhVMiCENtU");
        var response = await httpClient.GetAsync(string.Format(URL, searchTerm));

        if (response.IsSuccessStatusCode)
        {
            return PanvelSerializer.Deserialize(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return new List<Medicine>();
        }
    }
}

public class PanvelSerializer
{
    public static List<Medicine> Deserialize(string rawResult)
    {
        var medicines = new List<Medicine>();

        JsonElement items;
        if (!JsonSerializer.Deserialize<JsonElement>(rawResult).TryGetProperty("items", out items))
            return medicines;

        foreach (var item in items.EnumerateArray())
        {
            var newMedicine = new Medicine();
            newMedicine.Thumbnail = new UriImageSource() { Uri = new Uri(item.GetProperty("image").GetString()) };
            newMedicine.OldPrice = (float)item.GetProperty("originalPrice").GetDecimal();
            newMedicine.Price = (float)item.GetProperty("finalPrice").GetDecimal();
            newMedicine.Name = item.GetProperty("productFullName").GetString();
            newMedicine.DrugStore = "Panvel";

            medicines.Add(newMedicine);
        }

        return medicines;
    }
}



