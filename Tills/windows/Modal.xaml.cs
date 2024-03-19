using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

namespace ComputingEPOS.Tills;

public partial class Modal : UserControl
{
    public static Modal Instance => MainWindow.Instance.Modal;

    public static readonly DependencyProperty MessageProperty = DependencyProperty.RegisterAttached("Message", typeof(string), typeof(Modal), new FrameworkPropertyMetadata("Modal Message"));
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty HideOnClickProperty = DependencyProperty.RegisterAttached("HideOnClick", typeof(bool), typeof(Modal), new FrameworkPropertyMetadata(true));
    public bool HideOnClick
    {
        get => (bool)GetValue(HideOnClickProperty);
        set => SetValue(HideOnClickProperty, value);
    }

    public static readonly DependencyProperty VisibleProperty = DependencyProperty.RegisterAttached("Visible", typeof(bool), typeof(Modal), new FrameworkPropertyMetadata(false));
    public bool Visible
    {
        get => (bool)GetValue(VisibleProperty);
        set => SetValue(VisibleProperty, value);
    }

    public void ShowNotImplementedModal() => UIDispatcher.EnqueueUIAction(() => {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => Show("Not Implemented"));
    });

    public async Task ShowError(string error, HttpResponseMessage response, bool hideOnClick = true)
    {
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        string? reason = null;

        if (content != null) {
            if (content.ContainsKey("detail")) {
                reason = content["detail"].ToString();
            }
            else if (content.ContainsKey("title")) {
                reason = content["title"].ToString();
            }
        }

        StringBuilder sb = new(error);

        sb.Append(" (");
        sb.Append(response.StatusCode);
        sb.Append(")");
        sb.Append("\n\nReason: ");
        sb.Append(reason ?? "Unknown");

        if (response.StatusCode == HttpStatusCode.BadRequest && content != null && content.ContainsKey("errors")) {
            var errors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(content["errors"].ToString()!)!;

            foreach (var errorPair in errors) {
                sb.Append($"\n{errorPair.Key}: ");

                for (int i = 0; i < errorPair.Value.Count; i++) {
                    var e = errorPair.Value[i];

                    if (i > 0) sb.Append(" ");
                    sb.Append($"{e.ToString()}");
                }
            }
        }

        UIDispatcher.EnqueueOnUIThread(() => Show(sb.ToString()));
        UIDispatcher.UpdateUI();
    }

    public void Show(string message, bool hideOnClick = true) {
        Message = message;
        HideOnClick = hideOnClick;
        Visible = true;
    }

    public void Hide() => Visible = false;

    public Modal()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnClick(object sender, RoutedEventArgs e) {
        if (HideOnClick) Visible = false;
    }
}
