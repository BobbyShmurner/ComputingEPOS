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
}
