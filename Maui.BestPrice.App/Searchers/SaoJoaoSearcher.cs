using System;
using System.Text.Json;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class SaoJoaoSearcher : IMedicineSearcher
{
    const string URL = @"https://apiappprd.saojoaofarmacias.com.br/api/v2/products/linx?terms={0}";

    public SaoJoaoSearcher()
	{
	}

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(string.Format(URL, searchTerm));

        if (response.IsSuccessStatusCode)
        {
            //var medicinesRaw = (JsonElement)JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            //var items = JsonSerializer.Deserialize<JsonElement>(medicinesRaw.GetProperty("data"));

            //var medicines = new List<Medicine>();
            //foreach (var item in items.EnumerateArray())
            //{
            //    var rawItem = item.GetProperty("item");
            //    medicines.Add(new Medicine()
            //    {
            //        Name = rawItem.GetProperty("name").GetString(),
            //        DrugStore = "São João",
            //        Price = new Price() { Value = (float)rawItem.GetProperty("price").GetDecimal() }
            //    });
            //}

            return SaoJoaoSerializer.Deserialize(await response.Content.ReadAsStringAsync());

        } else
        {
            return default(IEnumerable<Medicine>);
        }
    }
}

public class SaoJoaoSerializer
{
    public static List<Medicine> Deserialize(string rawResult)
    {
        var medicines = new List<Medicine>();
        var medicinesRaw = JsonSerializer.Deserialize<JsonElement>(rawResult).GetProperty("data");
        foreach (var item in medicinesRaw.EnumerateArray())
        {
            var newMedicine = new Medicine();
            foreach (var image in item.GetProperty("imagens").EnumerateArray())
                newMedicine.Thumbnail = image.GetString();

            newMedicine.OldPrice = (float)item.GetProperty("precoDe").GetDecimal();

            foreach (var promotion in item.GetProperty("promocoes").EnumerateArray())
            {
                newMedicine.OldPrice = (float)promotion.GetProperty("valueFrom").GetDecimal();
                newMedicine.Price = (float)promotion.GetProperty("valueTo").GetDecimal();
                newMedicine.ExpireDate = promotion.GetProperty("dtFinal").GetDateTime();
            }

            newMedicine.Name = item.GetProperty("nomeProduto").GetString();
            newMedicine.DrugStore = "São João";

            medicines.Add(newMedicine);
        }

        return medicines;
    }
}

