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

public class Orders : Singleton<Client> {
    public static async Task<Order?> Create()
    {
        var item = new Order {
            EmployeeID = 1,
        };

        var returnedItem = await Client.PostAsync<Order>("api/Orders", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        return returnedItem;
    }

    public static async Task<decimal?> GetAmountPaid(Order order) =>
        await Client.GetAsync<decimal>($"api/Orders/{order.OrderID}/AmountPaid");
}
