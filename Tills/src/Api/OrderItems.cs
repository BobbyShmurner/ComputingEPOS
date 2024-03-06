using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ComputingEPOS.Models;

namespace ComputingEPOS.Tills.Api;

public static class OrderItems {
    public static async Task<OrderItem?> Create(int orderID, int stockID, int quantity, decimal? subtotal)
    {
        var item = new OrderItem
        {
            OrderID = orderID,
            StockID = stockID,
            Quantity = quantity,
            Subtotal = subtotal ?? 0M,
        };

        var response = await Client.PostAsync("api/OrderItems", new StringContent(
            $"[{JsonSerializer.Serialize(item)}]",
            Encoding.UTF8,
            "application/json"
        ));

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<OrderItem[]>())?[0];
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to create order item!", response);
            throw;
        }
    }

    public static async Task Delete(OrderItem item) {
        var response = await Client.DeleteAsync($"api/OrderItems/{item.OrderItemID}");
        
        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to delete order item!", response);
            throw;
        }
    }
}
