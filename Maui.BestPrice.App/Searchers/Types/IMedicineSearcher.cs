using System;
using Maui.BestPrice.App.Models;

namespace Maui.BestPrice.App.Searchers.Types;

public interface IMedicineSearcher
{
    Task<IEnumerable<Medicine>> SearchAsync(string searchTerm);
}

