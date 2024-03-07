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

public static class Orders {
    public static async Task<Order?> Create()
    {
        var item = new Order {
            EmployeeID = 1,
        };

        var response = await Client.PostAsync("api/Orders", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        try {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Order>();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to create order!", response);
            throw;
        }
    }

    public static async Task<Order> FinaliseOrder(Order order) {
        var response = await Client.PostAsync($"api/Orders/{order.OrderID}/Finalise", null);

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Order>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to finalise Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            throw;
        }
    }

    public static async Task<List<Order>> GetOpenChecks() {
        var response = await Client.GetAsync($"api/Orders?closed=false");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Order>>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get open checks!", response);
            throw;
        }
    }

    public static async Task ForceCloseAllChecks() {
        var response = await Client.PostAsync($"api/Orders/ForceCloseAllChecks", null);

        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to force close all checks!", response);
            throw;
        }
    }

    public static async Task CloseAllPaidChecks(bool closeEmpty) {
        var response = await Client.PostAsync($"api/Orders/CloseAllPaidChecks?closeEmpty={closeEmpty}", null);

        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to close all paid checks!", response);
            throw;
        }
    }

    public static async Task DeleteOrder(Order order) {
        var response = await Client.DeleteAsync($"api/Orders/{order.OrderID}");

        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to delete Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            throw;
        }
    }

    public static async Task<Order> CloseCheck(Order order, bool forceClose = false) {
        var response = await Client.PostAsync($"api/Orders/{order.OrderID}/CloseCheck?force={forceClose}", null);

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Order>())!;
        } catch (HttpRequestException ex) {
            if (ex.StatusCode != null) {
                await Modal.Instance.ShowError($"Failed to close Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            }
            throw;
        }
    }

    public static async Task<decimal?> GetAmountPaid(Order order) {
        var response = await Client.GetAsync($"api/Orders/{order.OrderID}/AmountPaid");

        try {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<decimal>();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get amount paid for Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            throw;
        }
    }
}
