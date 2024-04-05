using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for DbView.xaml
/// </summary>
public partial class DbView : UserControl
{
    public DataGrid DataGrid => DG_MainGrid;
    public DataGrid AddGrid => DG_AddGrid;

    public List<IDbGrid> DbGrids { get; }
    public IDbGrid? CurrentGrid { get; private set; }

    public DbView() {
        InitializeComponent();
        DataContext = this;

        DbGrids = new List<IDbGrid> {
            new EmployeeDbGrid(),
            new StockDbGrid(),
            new MenuDbGrid(),
            new MenuItemsDbGrid(),
            new Menu_MenuItemsDbGrid(),
        };

        GenerateDbButtons();
    }

    void GenerateDbButtons() {
        foreach (IDbGrid grid in DbGrids)
        {
            TextBlock buttonContent = new();
            buttonContent.Text = grid.Title;

            Button button = new();
            button.Content = buttonContent;

            button.Click += (_, _) => UIDispatcher.EnqueueUIAction(async () => {
                await ShowGrid(grid, resetSelection: true);
                UIDispatcher.UpdateUI();
            });

            ReportButtonsStackPanel.Children.Add(button);
        }
    }

    public Task RefreshCurrentGrid() {
        if (CurrentGrid == null) CurrentGrid = DbGrids[0];
        return ShowGrid(CurrentGrid, resetSelection: false);
    }

    public async Task ShowGrid(IDbGrid grid, bool resetSelection) {
        DataGrid? dataGrid = null;
        DataGrid? addGrid = null;

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            LoadingOverlay.Visibility = Visibility.Visible;

            dataGrid = DataGrid;
            addGrid = AddGrid;
        });

        CurrentGrid?.HideGrid();
        CurrentGrid = grid;
        await grid.ShowGrid(dataGrid!, addGrid!, DataLeft, DataCenter, DataRight, resetSelection);

        UIDispatcher.EnqueueOnUIThread(() => {
            LoadingOverlay.Visibility = Visibility.Collapsed;
        });
    }

    public void PrintCurrentGrid() {
        if (CurrentGrid == null) {
            UIDispatcher.EnqueueOnUIThread(() => Modal.Instance.Show("No data grid selected!"));
            return;
        }

        CurrentGrid.PrintGrid();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView))
    );

    private void PrintButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(PrintCurrentGrid)
    );

    private void SaveButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        IDbGrid? currentGrid = null;
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            if (CurrentGrid == null) return;

            currentGrid = CurrentGrid;
            Modal.Instance.Show("Saving...", false);
        });

        if (currentGrid == null) return;

        try {
            await currentGrid.SaveChanges();
            await MainWindow.Instance.MenuView.OrderMenuManager.RefreshMenusFromDB();
        } catch (Exception e) when (e is not HttpRequestException) {
            UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Show(e.Message));
            throw;
        }

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Hide());

        await RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        IDbGrid? currentGrid = null;
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            if (CurrentGrid == null) return;

            currentGrid = CurrentGrid;
            Modal.Instance.Show("Deleting...", false);
        });

        if (currentGrid == null) return;

        try {
            await currentGrid.DeleteCurrent();
            await MainWindow.Instance.MenuView.OrderMenuManager.RefreshMenusFromDB();
        } catch (Exception e) when (e is not HttpRequestException) {
            UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Show(e.Message));
            throw;
        }

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Hide());

        await RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void DG_Scroll(object sender, MouseWheelEventArgs e) {
        var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
            RoutedEvent = MouseWheelEvent,
        };

        SV_ReportScrollViewer.RaiseEvent(args);
    }
}
