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

public class OrderItems : Singleton<Client> {
    public static async Task<OrderItem?> Create(int orderID, int stockID, int quantity, decimal? subtotal)
    {
        var item = new OrderItem
        {
            OrderID = orderID,
            StockID = stockID,
            Quantity = quantity,
            Subtotal = subtotal ?? 0M,
        };

        var returnedItem = await Client.PostAsync<OrderItem[]>("api/OrderItems", new StringContent(
            $"[{JsonSerializer.Serialize(item)}]",
            Encoding.UTF8,
            "application/json"
        ));

        return returnedItem[0];
    }

    public static async Task Delete(OrderItem item)
    {
        await Client.DeleteAsync($"api/OrderItems/{item.OrderItemID}");
    }
}
