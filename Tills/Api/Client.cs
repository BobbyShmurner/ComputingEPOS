using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills.Api;

public class Client : Singleton<Client> {
    public event Action<HttpRequestException>? OnRequestException;

    public static HttpClient HttpClient { get; private set; } = new() {
        BaseAddress = new Uri("http://localhost:5068/api"),
    };

    public static async Task<HttpResponseMessage> GetAsync(string requestUri) {
        try {
            return await HttpClient.GetAsync(requestUri);
        } catch (HttpRequestException e) {
            Trace.WriteLine("GOT HTTP REQUEST EXCEPTION!!");
            Trace.WriteLine(Instance.OnRequestException?.GetInvocationList().Length);
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