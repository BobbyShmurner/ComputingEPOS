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

public static class MenuItems {
    public static async Task<List<MenuItem>> GetMenuItems()
    {
        var response = await Client.GetAsync($"api/MenuItems");

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<MenuItem>>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to get all Menu Items!", response);
            throw;
        }
    }

    public static async Task<MenuItem?> Create(MenuItem item)
    {
        var response = await Client.PostAsync("api/MenuItems", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MenuItem>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to create Menu Item!", response);
            throw;
        }
    }

    public static async Task<MenuItem> PutMenuItem(MenuItem menuItem) {
        var response = await Client.PutAsync($"api/MenuItems/{menuItem.MenuItemID}", new StringContent(
            JsonSerializer.Serialize(menuItem),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<MenuItem>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to update Menu Item [ID: {menuItem.MenuItemID}]", response);
            throw;
        }
    }

    public static async Task DeleteMenuItem(MenuItem menuItem)
    {
        var response = await Client.DeleteAsync($"api/MenuItems/{menuItem.MenuItemID}");

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to delete Menu Item [ID: {menuItem.MenuItemID}]", response);
            throw;
        }
    }
}
