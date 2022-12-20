using System;
using System.Text.Json;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class SaoJoaoSearcher : IMedicineSearcher
{
    const string URL = @"https://apiappprd.saojoaofarmacias.com.br/api/v2/products/linxAutocomplete?terms={0}";

    public SaoJoaoSearcher()
	{
	}

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(string.Format(URL, searchTerm));

        if (response.IsSuccessStatusCode)
        {
            var medicinesRaw = (JsonElement)JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            var items = JsonSerializer.Deserialize<JsonElement>(medicinesRaw.GetProperty("data").GetProperty("products"));

            var medicines = new List<Medicine>();
            foreach (var item in items.EnumerateArray())
            {
                var rawItem = item.GetProperty("item");
                medicines.Add(new Medicine()
                {
                    Name = rawItem.GetProperty("name").GetString(),
                    DrugStore = "São João",
                    Price = new Price() { Value = (float)rawItem.GetProperty("price").GetDecimal() }
                });
            }

            return medicines;

        } else
        {
            return default(IEnumerable<Medicine>);
        }
    }
}

