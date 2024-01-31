using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ComputingEPOS.Tills.Api;

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for ConnectionScreen.xaml
/// </summary>
public partial class ConnectionScreen : UserControl {
    public bool ConnectionUp { get; private set; } = true;
    public MainWindow? Window { get; private set; }

    FrameworkElement? previousView;
    DispatcherTimer retryTimer;

    public ConnectionScreen() {
        InitializeComponent();

        retryTimer = new DispatcherTimer();
        retryTimer.Interval = TimeSpan.FromSeconds(3);
        retryTimer.Tick += (_, _) => Ping();

        Client.Instance.OnRequestException += OnRequestException;
    }

    void OnRequestException(HttpRequestException e) {
        Trace.WriteLine($"Http Error Message: {e.Message}");
        Trace.WriteLine($"Http Error Code: {e.StatusCode}");
        Trace.WriteLine($"Http Error Type: {e.InnerException?.GetType() ?? e.GetType()}");
        if (e.StatusCode == null) SetConnectionDown();
    }

    void CacheWindow() {
        if (Window != null) return;

        FrameworkElement? parent = this;
        do parent = parent.Parent as FrameworkElement;
        while (!parent!.GetType().IsAssignableTo(typeof(MainWindow)));

        Window = (MainWindow)parent;
    }

    public void Ping() {
        Task.Run(async () => {
            try {
                Trace.WriteLine("Ping!");
                var response = await Client.HttpClient.GetAsync("api/ping");
                response.EnsureSuccessStatusCode();

                string msg = await response.Content.ReadAsStringAsync();
                Trace.WriteLine($"Got Response: {msg}");

                if (!ConnectionUp) SetConnectionUp();
            } catch {
                SetConnectionDown();
                throw;
            }
        });
    }

    public void SetConnectionDown() {
        ConnectionUp = false;
        CacheWindow();

        if (Window!.RootViewManager.CurrentView != this)
            previousView = Window.RootViewManager.CurrentView;

        Dispatcher.Invoke(() => {
            Window.RootViewManager.ShowView(this);
            Window.Modal.Show("Connection to the Tills Server Lost!\n:(\n\nRetrying...", false);
        });

        StartRetryTimer();
    }

    void SetConnectionUp() {
        ConnectionUp = true;
        CacheWindow();
        Dispatcher.Invoke(() => {
            Window!.Modal.Hide();
            Window!.RootViewManager.ShowView(previousView);
        });
        retryTimer.Stop();
    }

    void StartRetryTimer() {
        if (retryTimer.IsEnabled) return;
        retryTimer.Start();
    }
}
