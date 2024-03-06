using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using ComputingEPOS.Models;

namespace ComputingEPOS.Tills.Api;

public static class Stock {
    public static async Task<List<PmixReport>> GetAllStockPmix(DateTime? from = null, DateTime? to = null) {
        var response = await Client.GetAsync($"api/Stock/Pmix?from={from?.ToUniversalTime().ToString("r")}&to={to?.ToUniversalTime().ToString("r")}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<PmixReport>>())!;
    }
}
