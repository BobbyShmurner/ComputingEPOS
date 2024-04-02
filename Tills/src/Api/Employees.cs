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

public static class Employees {
    public static async Task<List<Employee>> GetEmployees() {
        var response = await Client.GetAsync($"api/Employees");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Employee>>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get all Employees!", response);
            throw;
        }
    }

    public static async Task<Employee?> GetEmployeeFromPin(string pin) {
        var response = await Client.GetAsync($"api/Employees/FromPin?pin={pin}");

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Employee>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
            return null;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to get Employee from pin!", response);
            throw;
        }
    }

    public static async Task<Employee?> Create(Employee item) {
        var response = await Client.PostAsync("api/Employees", new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json"
        ));

        try {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Employee>();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to create employee!", response);
            throw;
        }
    }

    public static async Task<Employee> PutEmployee(Employee employee) {
        var response = await Client.PutAsync($"api/Employees/{employee.EmployeeID}", new StringContent(
            JsonSerializer.Serialize(employee),
            Encoding.UTF8,
            "application/json"
        ));

        try {
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Employee>())!;
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to update Employee!", response);
            throw;
        }
    }

    public static async Task DeleteEmployee(Employee employee) {
        var response = await Client.DeleteAsync($"api/Employees/{employee.EmployeeID}");

        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException ex) when (ex.StatusCode != null) {
            await Modal.Instance.ShowError($"Failed to delete Employee [ID: {employee.EmployeeID}]", response);
            throw;
        }
    }
}
