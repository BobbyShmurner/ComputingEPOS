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