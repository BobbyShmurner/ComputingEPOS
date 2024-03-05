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
    public DataGrid DataGrid { get; private set; }

    public List<IReportGrid> ReportGrids { get; }

    public ReportsView() {
        InitializeComponent();
        DataContext = this;

        DataGrid = MainGrid;
        ReportGrids = new List<IReportGrid> {
            new SalesReportGrid()
        };
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
        CurrentGrid = grid;

        DataGrid? dataGrid = null;
        TimeInterval? interval = null;

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            dataGrid = DataGrid;
            interval = Interval;
        });

        await grid.ShowGrid(dataGrid!, interval!.Value);

        //UIDispatcher.EnqueueOnUIThread(() =>
        //{
        //    EmptyGrid.Columns.Clear();

        //    for (int i = 0; i < DataGrid.Columns.Count; i++)
        //    {
        //        EmptyGrid.Columns.Add(new DataGridTextColumn
        //        {
        //            Width = DataGrid.Columns[i].Width
        //        });
        //    }

        //    var items = new ObservableCollection<byte>();

        //    int itemCount = 0;
        //    foreach (var item in DataGrid.ItemsSource)
        //    {
        //        itemCount++;
        //    }

        //    for (int i = 0; i < 20; i++)
        //    {
        //        items.Add(0);
        //    }

        //    EmptyGrid.ItemsSource = items;
        //});
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView))
    );

    private void PrintButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();

    private void SwitchReportButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        await ShowGrid(ReportGrids[0]);
        UIDispatcher.UpdateUI();
    });
}
