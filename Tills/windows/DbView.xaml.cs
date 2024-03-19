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
/// Interaction logic for DbView.xaml
/// </summary>
public partial class DbView : UserControl
{
    public DataGrid DataGrid => DG_MainGrid;

    public List<IDbGrid> DbGrids { get; }
    public IDbGrid? CurrentGrid { get; private set; }

    public DbView() {
        InitializeComponent();
        DataContext = this;

        DbGrids = new List<IDbGrid> {
            new EmployeeDbGrid()
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

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            LoadingOverlay.Visibility = Visibility.Visible;

            dataGrid = DataGrid;
            grid.HideGrid(dataGrid!);
        });

        CurrentGrid = grid;
        await grid.ShowGrid(dataGrid!, DataLeft, DataCenter, DataRight, resetSelection);

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

    private void SaveButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        IDbGrid? currentGrid = null;
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            if (CurrentGrid == null) return;

            currentGrid = CurrentGrid;
            Modal.Instance.Show("Saving...", false);
        });

        if (currentGrid == null) return;
        await currentGrid.SaveChanges();

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Hide());

        await RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        Modal.Instance.ShowNotImplementedModal()
    );

    private void DG_Scroll(object sender, MouseWheelEventArgs e) {
        var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
            RoutedEvent = MouseWheelEvent,
        };

        SV_ReportScrollViewer.RaiseEvent(args);
    }
}
