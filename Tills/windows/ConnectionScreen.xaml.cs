﻿using System;
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

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for ConnectionScreen.xaml
/// </summary>
public partial class ConnectionScreen : UserControl {
    public static ConnectionScreen Instance => MainWindow.Instance.ConnectionScreen;

    public static bool ConnectionUp { get; private set; } = false;

    FrameworkElement? previousView;

    public ConnectionScreen() {
        InitializeComponent();
    }

    /// <summary>
    /// Freezes execution until connected to the server.
    /// </summary>
    /// <returns></returns>
    public async Task EnsureConnected(bool setViewOnDown = true, bool setViewOnUp = true) {
        if (ConnectionUp) return;
        
        while (!ConnectionUp) {
            await Ping(setViewOnDown, setViewOnUp);
            await Task.Delay(1000);
        }
    }

    /// <summary>
    /// Shows the last screen displayed before switching to the connection screen.
    /// </summary>
    public void ShowPreviousScreen() {
        UIDispatcher.DispatchOnUIThread(() => {
            MainWindow.Instance.Modal.Hide();
            MainWindow.Instance.RootViewManager.ShowView(previousView);
        });
    }

    /// <summary>
    /// Ping the server. Sets the connection down if the ping fails.
    /// </summary>
    public async Task Ping(bool setViewOnDown = true, bool setViewOnUp = true) {
        try {
            Trace.WriteLine("Ping!");

            // Make sure to call `HttpClient.GetAsync` instead of `Client.GetAsync`
            // To avoid the timeout exception being caught by the client
            var response = await Api.Client.HttpClient.GetAsync("api/ping");
            response.EnsureSuccessStatusCode();

            string msg = await response.Content.ReadAsStringAsync();
            Trace.WriteLine($"Got Response: {msg}");

            if (!ConnectionUp) SetConnectionUp(setViewOnUp);
        } catch {
            SetConnectionDown(setViewOnDown);
        }
    }

    /// <summary>
    /// Signifies that the tills cannot communicate with the server.
    /// </summary>
    /// <param name="updateView"></param>
    public void SetConnectionDown(bool updateView) {
        ConnectionUp = false;
        if (!updateView) return;

        UIDispatcher.DispatchOnUIThread(() => {
            var currentView = MainWindow.Instance.RootViewManager.CurrentView;

            if (currentView != this)
                previousView = currentView;

            MainWindow.Instance.RootViewManager.ShowView(this);
            MainWindow.Instance.Modal.Show("Connection to the Tills Server Lost!\n\nRetrying...", false);
        });
    }

    /// <summary>
    /// Signifies that the tills have regained connection to the server.
    /// </summary>
    /// <param name="updateView"></param>
    void SetConnectionUp(bool updateView) {
        ConnectionUp = true;

        if (!updateView) return;
        ShowPreviousScreen();
    }
}
