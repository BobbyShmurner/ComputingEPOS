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

public static class Menu_MenuItems {
    public static async Task<List<Menu_MenuItem>> GetMenu_MenuItems()
    {
        var response = await Client.GetAsync($"api/Menu_MenuItems");

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Menu_MenuItem>>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to get all Menu_MenuItems!", response);
            throw;
        }
    }

    public static async Task<Menu_MenuItem?> Create(Menu_MenuItem menu_MenuItem)
    {
        var response = await Client.PostAsync("api/Menu_MenuItems", new StringContent(
            JsonSerializer.Serialize(menu_MenuItem),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Menu_MenuItem>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to create Menu_MenuItem!", response);
            throw;
        }
    }

    public static async Task<Menu_MenuItem> PutMenu_MenuItem(Menu_MenuItem menu_MenuItem) {
        var response = await Client.PutAsync($"api/Menu_MenuItems/{menu_MenuItem.MenuID}", new StringContent(
            JsonSerializer.Serialize(menu_MenuItem),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Menu_MenuItem>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to update Menu_MenuItem [ID: {menu_MenuItem.MenuID}]", response);
            throw;
        }
    }

    public static async Task DeleteMenu_MenuItem(Menu_MenuItem menu_MenuItem)
    {
        var response = await Client.DeleteAsync($"api/Menu_MenuItems/{menu_MenuItem.MenuID}");

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to delete Menu_MenuItem [ID: {menu_MenuItem.MenuID}]", response);
            throw;
        }
    }
}
