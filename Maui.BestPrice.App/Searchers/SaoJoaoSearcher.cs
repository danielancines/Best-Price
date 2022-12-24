using System;
using System.Text.Json;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class SaoJoaoSearcher : IMedicineSearcher
{
    const string URL = @"https://apiappprd.saojoaofarmacias.com.br/api/v2/products/linx?terms={0}";

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(string.Format(URL, searchTerm));

        if (response.IsSuccessStatusCode)
        {
            return SaoJoaoSerializer.Deserialize(await response.Content.ReadAsStringAsync());

        }
        else
        {
            return new List<Medicine>();
        }
    }
}

public class SaoJoaoSerializer
{
    public static List<Medicine> Deserialize(string rawResult)
    {
        var medicines = new List<Medicine>();

        JsonElement data;
        if (!JsonSerializer.Deserialize<JsonElement>(rawResult).TryGetProperty("data", out data))
            return medicines;

        if (data.ValueKind != JsonValueKind.Array)
            return medicines;

        foreach (var item in data.EnumerateArray())
        {
            var newMedicine = new Medicine();
            foreach (var image in item.GetProperty("imagens").EnumerateArray())
                newMedicine.Thumbnail = image.GetString();

            newMedicine.Price = (float)item.GetProperty("precoDe").GetDecimal();

            JsonElement priceFor = item.GetProperty("precoPor");
            if (priceFor.ValueKind != JsonValueKind.Null)
                newMedicine.Price = (float)priceFor.GetDecimal();

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

