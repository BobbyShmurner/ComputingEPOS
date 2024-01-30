using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        Client.Instance.OnRequestException += _ => SetConnectionDown();
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
                var response = await Client.GetAsync<string>("api/ping");
                Trace.WriteLine(response);

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

        Dispatcher.Invoke(() => Window.RootViewManager.ShowView(this));

        StartRetryTimer();
    }

    void SetConnectionUp() {
        ConnectionUp = true;
        CacheWindow();
        Dispatcher.Invoke(() => Window!.RootViewManager.ShowView(previousView));
        retryTimer.Stop();
    }

    void StartRetryTimer() {
        if (retryTimer.IsEnabled) return;
        retryTimer.Start();
    }
}
