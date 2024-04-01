using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ComputingEPOS.Models;
using static ComputingEPOS.Models.Transaction;

namespace ComputingEPOS.Tills.Api;

public static class Transactions {
    public static async Task<Transaction?> Create(Order order, decimal amountPaid, PaymentMethods paymentMethod)
    {
        var item = new Transaction
        {
            OrderID = order.OrderID,
            AmountPaid = amountPaid,
            Method = paymentMethod,
        };

        var response = await Client.PostAsync("api/Transactions", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        try {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Transaction>();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to create transaction for Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            throw;
        }
    }

    public static async Task<List<Transaction>> GetTransactions(Order? order) {
        var response = await Client.GetAsync($"api/Transactions?orderId={order?.OrderID}");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Transaction>>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get transactions!", response);
            throw;
        }
    }

    public static async Task<List<decimal>> GetGrossSalesInIntervals(DateTime from, DateTime? to, long intervalInSeconds)
    {
        var response = await Client.GetAsync($"api/Transactions/GrossSalesInIntervals?from={from.ToUniversalTime():r}&to={to?.ToUniversalTime().ToString("r")}&intervalInSeconds={intervalInSeconds}");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<decimal>>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get the gross sales!", response);
            throw;
        }
    }
}
