using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Resources;
using Maui.BestPrice.App.Searchers.Types;
using System.Text;
using System.Text.Json;

namespace Maui.BestPrice.App.Searchers;

public class DrogaRaiaSearcher : IMedicineSearcher
{
    const string URL = @"https://app-api-m2-prod.drogaraia.com.br/graphql";

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", "5f308895c59fadb0b9ed43341c6eb33e41e78394d3ca970c5a285e91d25bc9cd");

        var content = new StringContent(SearchUrls.DrogaRaia.Replace("!searchTerm!", searchTerm), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(string.Format(URL, searchTerm), content);

        if (response.IsSuccessStatusCode)
        {
            return DrogaraiaSerializer.Deserialize(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return new List<Medicine>();
        }
    }
}

public class DrogaraiaSerializer
{
    public static List<Medicine> Deserialize(string rawResult)
    {
        var medicines = new List<Medicine>();

        JsonElement data;
        if (!JsonSerializer.Deserialize<JsonElement>(rawResult).TryGetProperty("data", out data))
            return medicines;

        JsonElement search;
        if (!JsonSerializer.Deserialize<JsonElement>(rawResult).TryGetProperty("search", out search))
            return medicines;

        JsonElement products;
        if (!JsonSerializer.Deserialize<JsonElement>(rawResult).TryGetProperty("products", out products))
            return medicines;

        foreach (var item in products.EnumerateArray())
        {
            var newMedicine = new Medicine();

            newMedicine.Name = item.GetProperty("name").GetString();
            newMedicine.OldPrice = (float)item.GetProperty("oldPrice").GetProperty("value").GetDecimal();
            newMedicine.Price = (float)item.GetProperty("price").GetProperty("value").GetDecimal();
            newMedicine.DrugStore = "Droga Raia";
            newMedicine.Thumbnail = item.GetProperty("image").GetString();

            medicines.Add(newMedicine);
        }

        return medicines;
    }
}

