﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills.Api;

public class Client : Singleton<Client> {
    public event Action? OnTimeout;

    public static HttpClient HttpClient { get; private set; } = new() {
        BaseAddress = new Uri("http://localhost:5068/api"),
    };

    public static Task<HttpResponseMessage> GetAsync(string? requestUri) =>
        Instance.GenericRequestAsync(() => HttpClient.GetAsync(requestUri));

    public static Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content) =>
        Instance.GenericRequestAsync(() => HttpClient.PostAsync(requestUri, content));

    public static Task<HttpResponseMessage> PutAsync(string? requestUri, HttpContent? content) =>
        Instance.GenericRequestAsync(() => HttpClient.PutAsync(requestUri, content));

    public static Task<HttpResponseMessage> DeleteAsync(string? requestUri) =>
        Instance.GenericRequestAsync(() => HttpClient.DeleteAsync(requestUri));

    async Task<T> GenericRequestAsync<T>(Func<Task<T>> taskFactory) {
        Trace.WriteLine("Executing Async Request");
        bool didTimeout = false;

        while (true) {
            try {
                T res = await taskFactory();

                if (didTimeout) {
                    UIDispatcher.DispatchOnUIThread(() => {
                        ConnectionScreen.Instance.ShowPreviousScreen();
                        Modal.Instance.Hide();
                    });
                }

                return res;
            } catch (Exception ex) when (
                ex is HttpRequestException && ((HttpRequestException)ex).StatusCode == null ||
                ex is TaskCanceledException && ex.InnerException is TimeoutException
            ) {
                didTimeout = true;
                Instance.OnTimeout?.Invoke();

                ConnectionScreen.Instance.SetConnectionDown(true);
                await ConnectionScreen.Instance.EnsureConnected(false, false);
            }
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