using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        DataGrid = MainGrid;
        ReportGrids = new List<IReportGrid> {
            new SalesReportGrid()
        };

        ShowGrid(ReportGrids[0]);
    }

    public void ShowGrid(IReportGrid grid) {
        grid.ShowGrid(DataGrid);
        EmptyGrid.Columns.Clear();

        for( int i = 0; i < DataGrid.Columns.Count; i++ )
        {
            EmptyGrid.Columns.Add(new DataGridTextColumn
            {
                Width = DataGrid.Columns[i].Width
            });
        }

        var items = new ObservableCollection<byte>();

        int itemCount = 0;
        foreach (var item in DataGrid.ItemsSource) {
            itemCount++;
        }

        for (int i = 0; i < 20; i++)
        {
            items.Add(0);
        }

        EmptyGrid.ItemsSource = items;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView))
    );

    private void PrintButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void SwitchReportButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
}
