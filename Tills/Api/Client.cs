using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills.Api;

public class Client : Singleton<Client> {
    public event Action<HttpRequestException>? OnRequestException;

    public static HttpClient HttpClient { get; private set; } = new() {
        BaseAddress = new Uri("http://localhost:5068/api"),
    };

    public static async Task<T?> GetAsync<T>(string requestUri) {
        try {
            var response = await HttpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        } catch (HttpRequestException e) {
            Instance.OnRequestException?.Invoke(e);
            throw;
        }
    }

    public static async Task<T?> PostAsync<T>(string postUri, HttpContent? content)
    {
        try
        {
            var response = await HttpClient.PostAsync(postUri, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException e)
        {
            Instance.OnRequestException?.Invoke(e);
            throw;
        }
    }

    public static async Task DeleteAsync(string requestUri)
    {
        try
        {
            var response = await HttpClient.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Instance.OnRequestException?.Invoke(e);
            throw;
        }
    }
}

static class HttpResponseMessageExtensions {
    internal static void WriteRequestToConsole(this HttpResponseMessage response) {
        if (response is null) return;

        var request = response.RequestMessage;
        Trace.Write($"{request?.Method} ");
        Trace.Write($"{request?.RequestUri} ");
        Trace.WriteLine($"HTTP/{request?.Version}");
    }
}