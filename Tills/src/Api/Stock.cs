using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ComputingEPOS.Models;

namespace ComputingEPOS.Tills.Api;

public static class Stock {
    public static async Task<List<PmixReport>> GetAllStockPmix(DateTime? from = null, DateTime? to = null) {
        var response = await Client.GetAsync($"api/Stock/Pmix?from={from?.ToUniversalTime().ToString("r")}&to={to?.ToUniversalTime().ToString("r")}");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<PmixReport>>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get Pmix Report!", response);
            throw;
        }
    }

    public static async Task<List<Models.Stock>> GetStock()
    {
        var response = await Client.GetAsync($"api/Stock");

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Models.Stock>>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to get all Stock!", response);
            throw;
        }
    }

    public static async Task<Models.Stock?> Create(Models.Stock item)
    {
        var response = await Client.PostAsync("api/Stock", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Models.Stock>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to create stock!", response);
            throw;
        }
    }

    public static async Task<Models.Stock> PutStock(Models.Stock stock)
    {
        var response = await Client.PutAsync($"api/Stock/{stock.StockID}", new StringContent(
            JsonSerializer.Serialize(stock),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Models.Stock>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to update Stock!", response);
            throw;
        }
    }

    public static async Task DeleteStock(Models.Stock stock)
    {
        var response = await Client.DeleteAsync($"api/Stock/{stock.StockID}");

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to delete Stock [ID: {stock.StockID}]", response);
            throw;
        }
    }
}
