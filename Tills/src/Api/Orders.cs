﻿using System;
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

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Order>();
    }

    public static async Task FinaliseOrder(Order order) =>
        (await Client.PostAsync($"api/Orders/{order.OrderID}/Finalise", null)).EnsureSuccessStatusCode();

    public static async Task<List<Order>> GetOpenChecks() {
        var response = await Client.GetAsync($"api/Orders?closed=false");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Order>>())!;
    }

    public static async Task ForceCloseAllChecks() =>
        (await Client.PostAsync($"api/Orders/ForceCloseAllChecks", null)).EnsureSuccessStatusCode();

    public static async Task CloseAllPaidChecks(bool closeEmpty) =>
        (await Client.PostAsync($"api/Orders/CloseAllPaidChecks?closeEmpty={closeEmpty}", null)).EnsureSuccessStatusCode();

    public static async Task DeleteOrder(Order order) {
        var response = await Client.DeleteAsync($"api/Orders/{order.OrderID}");

        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to delete Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            throw;
        }
    }

    public static async Task CloseCheck(Order order, bool forceClose = false) {
        var response = await Client.PostAsync($"api/Orders/{order.OrderID}/CloseCheck?force={forceClose}", null);

        try {
            response.EnsureSuccessStatusCode();
            return;
        } catch (HttpRequestException ex) {
            if (ex.StatusCode != null) {
                await Modal.Instance.ShowError($"Failed to close Order #{order.OrderNum} [ID: {order.OrderID}]", response);
            }
            throw;
        }
    }

    public static async Task<decimal?> GetAmountPaid(Order order) =>
        await (await Client.GetAsync($"api/Orders/{order.OrderID}/AmountPaid")).Content.ReadFromJsonAsync<decimal>();
}
