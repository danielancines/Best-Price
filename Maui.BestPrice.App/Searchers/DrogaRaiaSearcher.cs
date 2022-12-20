using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Resources;
using Maui.BestPrice.App.Searchers.Types;
using System.Text;
using System.Text.Json;

namespace Maui.BestPrice.App.Searchers;

public class DrogaRaiaSearcher : IMedicineSearcher
{
    const string URL = @"https://app-api-m2-prod.drogaraia.com.br/graphql";

    public DrogaRaiaSearcher()
    {
    }

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "iOS");
        httpClient.DefaultRequestHeaders.Add("x-api-key", "5f308895c59fadb0b9ed43341c6eb33e41e78394d3ca970c5a285e91d25bc9cd");

        var content = new StringContent(SearchUrls.DrogaRaia.Replace("!searchTerm!", searchTerm), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(string.Format(URL, searchTerm), content);

        if (response.IsSuccessStatusCode)
        {
            //var a = await response.Content.ReadAsStringAsync();
            var medicinesRaw = (JsonElement)JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            var medicines = JsonSerializer.Deserialize<IList<Medicine>>(medicinesRaw.GetProperty("data").GetProperty("search").GetProperty("products"));

            foreach (var medicine in medicines)
            {
                medicine.DrugStore = "Droga Raia";
            }

            return medicines;

        }
        else
        {
            return default(IEnumerable<Medicine>);
        }
    }
}

