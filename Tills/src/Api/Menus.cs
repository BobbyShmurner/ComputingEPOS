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

using Menu = ComputingEPOS.Models.Menu;

public static class Menus {
    public static async Task<List<Menu>> GetMenus(bool? visible = null)
    {
        var response = await Client.GetAsync($"api/Menus?visible={visible}");

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Menu>>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to get all Menus!", response);
            throw;
        }
    }

    public static async Task<Menu?> Create(Menu menu)
    {
        var response = await Client.PostAsync("api/Menus", new StringContent(
            JsonSerializer.Serialize(menu),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Menu>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to create Menu!", response);
            throw;
        }
    }

    public static async Task<Menu> PutMenu(Menu menu) {
        var response = await Client.PutAsync($"api/Menus/{menu.MenuID}", new StringContent(
            JsonSerializer.Serialize(menu),
            Encoding.UTF8,
            "application/json"
        ));

        try
        {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Menu>())!;
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to update Menu [ID: {menu.MenuID}]", response);
            throw;
        }
    }

    public static async Task DeleteMenu(Menu menu)
    {
        var response = await Client.DeleteAsync($"api/Menus/{menu.MenuID}");

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode != null)
        {
            await Modal.Instance.ShowError($"Failed to delete Menu [ID: {menu.MenuID}]", response);
            throw;
        }
    }
}
