using System;
using System.Text.Json;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class PrecoPopularSearcher : IMedicineSearcher
{
    const string URL = @"https://api-gateway-prod.drogasil.com.br/search/v2/store/DROGARAIA/channel/SITE/product/search/live?term={0}&limit=4&sort_by=relevance:desc&origin=searchSuggestion";

    public PrecoPopularSearcher()
	{
	}

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "iOS");
        var response = await httpClient.GetAsync(string.Format(URL, searchTerm));

        if (response.IsSuccessStatusCode)
        {
            var medicinesRaw = (JsonElement)JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            var medicines = JsonSerializer.Deserialize<IList<Medicine>>(medicinesRaw.GetProperty("results").GetProperty("products"));

            foreach (var medicine in medicines)
            {
                medicine.DrugStore = "Preço Popular";
            }

            return medicines;

        } else
        {
            return default(IEnumerable<Medicine>);
        }
    }
}

