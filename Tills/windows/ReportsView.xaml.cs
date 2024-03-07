using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for ReportsView.xaml
/// </summary>
public partial class ReportsView : UserControl
{
    public DataGrid DataGrid => DG_MainGrid;
    public TotalGrid TotalGrid { get; }

    public List<IReportGrid> ReportGrids { get; }

    public ReportsView() {
        InitializeComponent();
        DataContext = this;

        ReportGrids = new List<IReportGrid> {
            new SalesReportGrid(),
            new ProductMixReportGrid(),
        };

        TotalGrid = new(DG_TotalGrid);

        GenerateReportButtons();
    }

    void GenerateReportButtons() {
        foreach (IReportGrid grid in ReportGrids)
        {
            TextBlock buttonContent = new();
            buttonContent.Text = grid.Title;

            Button button = new();
            button.Content = buttonContent;

            button.Click += (_, _) => UIDispatcher.EnqueueUIAction(async () => {
                await ShowGrid(grid);
                UIDispatcher.UpdateUI();
            });

            ReportButtonsStackPanel.Children.Add(button);
        }
    }

    public readonly DependencyProperty IntervalDependecyProperty = DependencyProperty.Register(
        nameof(Interval),
        typeof(TimeInterval),
        typeof(ReportsView),
        new PropertyMetadata(TimeInterval.Hourly, (d, e) => ((ReportsView)d).Interval = (TimeInterval)e.NewValue)
    );

    public TimeInterval Interval
    {
        get => (TimeInterval)GetValue(IntervalDependecyProperty);
        private set {
            SetValue(IntervalDependecyProperty, value);
            UIDispatcher.EnqueueUIAction(async () => {
                await RefreshCurrentGrid();
                UIDispatcher.UpdateUI();
            });
        }
    }

    public readonly DependencyProperty IntervalsDependecyProperty = DependencyProperty.Register(
        nameof(Intervals),
        typeof(TimeInterval[]),
        typeof(ReportsView),
        new PropertyMetadata(Enum.GetValues<TimeInterval>())
    );

    public TimeInterval[] Intervals => (TimeInterval[])GetValue(IntervalDependecyProperty);

    public IReportGrid? CurrentGrid { get; private set; }

    public Task RefreshCurrentGrid() {
        if (CurrentGrid == null) CurrentGrid = ReportGrids[0];
        return ShowGrid(CurrentGrid);
    }

    public async Task ShowGrid(IReportGrid grid) {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            LoadingOverlay.Visibility = Visibility.Visible;
        });

        CurrentGrid = grid;

        DataGrid? dataGrid = null;
        TimeInterval? interval = null;

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            dataGrid = DataGrid;
            interval = Interval;
        });

        await grid.ShowGrid(dataGrid!, TotalGrid, interval!.Value);

        UIDispatcher.EnqueueOnUIThread(() => {
            LoadingOverlay.Visibility = Visibility.Collapsed;
        });
    }

    public void PrintCurrentReport() {
        if (CurrentGrid == null) {
            Modal.Instance.Show("No report selected");
            return;
        }

        PrintManager.PrintString($"This is a {CurrentGrid.Title} Report :)", $"{CurrentGrid.Title} Report");
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView))
    );

    private void PrintButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(PrintCurrentReport)
    );

    private void DG_Scroll(object sender, MouseWheelEventArgs e) {
        var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
            RoutedEvent = MouseWheelEvent,
        };

        SV_ReportScrollViewer.RaiseEvent(args);
    }
}
