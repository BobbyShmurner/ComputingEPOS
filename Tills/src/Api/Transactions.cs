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

public class Transactions : Singleton<Client> {
    public static async Task<Transaction?> Create(Order order, decimal amountPaid, PaymentMethods paymentMethod)
    {
        var item = new Transaction
        {
            OrderID = order.OrderID,
            AmountPaid = amountPaid,
            Method = paymentMethod.ToString(),
        };

        var response = await Client.PostAsync("api/Transactions", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Transaction>();
    }

    public static async Task<List<decimal>> GetGrossSalesInIntervals(DateTime from, DateTime? to, long intervalInSeconds)
    {
        var response = await Client.GetAsync($"api/Transactions/GrossSalesInIntervals?from={from.ToUniversalTime():r}&to={to?.ToUniversalTime().ToString("r")}&intervalInSeconds={intervalInSeconds}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<decimal>>())!;
    }
}
