﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

using ComputingEPOS.Common.Models;
namespace ComputingEPOS.Tills;

public partial class MenuView : UserControl
{
    public const int SCROLL_AMOUNT = 100;

    public OrderManager OrderManager { get; private set; }
    public OrderMenuManager OrderMenuManager { get; private set; }
    public LoginManager LoginManager => MainWindow.Instance.LoginScreen.Manager;

    public MenuView()
    {
        InitializeComponent();
        this.DataContext = this;

        OrderMenuManager = new OrderMenuManager(this);
        Task.Run(async () => {
            await OrderMenuManager.RefreshMenusFromDB();
            UIDispatcher.UpdateUI();
        });

        OrderManager = new(this);
    }

    #region ShowScrollButtons Dependency Property
    public static readonly DependencyProperty ShowScrollButtonsProperty = DependencyProperty.Register("ShowScrollButtons", typeof(Visibility), typeof(MenuView), new UIPropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnShowScrollButtonsChanged), new CoerceValueCallback(OnCoerceShowScrollButtons)));

    private static object OnCoerceShowScrollButtons(DependencyObject o, object value)
    {
        if (o == null) return value;
        MenuView menuView = (MenuView)o;
        return menuView.OnCoerceShowScrollButtons((Visibility)value);
    }

    private static void OnShowScrollButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o == null) return;
        MenuView menuView = (MenuView)o;

        menuView.OnShowScrollButtonsChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
    }

    protected virtual Visibility OnCoerceShowScrollButtons(Visibility value)
    {
        return value;
    }

    protected virtual void OnShowScrollButtonsChanged(Visibility oldValue, Visibility newValue)
    {
        OrderManager.Selected?.BringIntoView();
    }

    public Visibility ShowScrollButtons
    {
        get => (Visibility)GetValue(ShowScrollButtonsProperty);
        set => SetValue(ShowScrollButtonsProperty, value);
    }

    private void SV_Orders_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ShowScrollButtons = e.ViewportHeight > e.ExtentHeight ? Visibility.Collapsed : Visibility.Visible;
    }

    #endregion

    private void Orders_SV_Up(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset - SCROLL_AMOUNT));
    private void Orders_SV_Down(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset + SCROLL_AMOUNT));

    private void Button_Clear(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.DeleteAllItems(true); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.RemoveSelectedOrderItem(true); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void SitInButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.CheckoutOrder(CheckoutType.SitIn); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void TakeAwayButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.CheckoutOrder(CheckoutType.TakeAway); }
        finally { UIDispatcher.UpdateUI(); }
    });


    private void OrderItemsEmptyFillButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => {
        try { OrderManager.DeselectItem(); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void TransactionButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        decimal? amount = null;
        Transaction.PaymentMethods paymentMethod = Transaction.PaymentMethods.Cash;
        TransactionButton.SpecialFunctions specialFunction = TransactionButton.SpecialFunctions.None;

        UIDispatcher.DispatchOnUIThread(() => {
            amount = TransactionButton.GetAmount((UIElement)sender);
            paymentMethod = TransactionButton.GetPaymentMethod((UIElement)sender);
            specialFunction = TransactionButton.GetSpecial((UIElement)sender);
        });

        try { await OrderManager.PayForOrder(amount, paymentMethod, specialFunction); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void ComboButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.MakeSelectedCombo(); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void ModifyButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void FunctionsButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
    {
        OrderMenuManager.ShowFunctionsScreen();
        UIDispatcher.UpdateUI();
    });

    private void ForceCloseAllChecks_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        await OrderManager.ForceCloseAllChecks();
        UIDispatcher.UpdateUI();
    });

    private void PowerOffButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        MainWindow.Shutdown(restart: false)
    );

    private void RebootButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        MainWindow.Shutdown(restart: true)
    );

    private void PrintReceiptButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => {
        try { OrderManager.PrintReceipt(); }
        catch (Exception e) { UIDispatcher.EnqueueOnUIThread(() => 
            Modal.Instance.Show($"Failed to print receipt!\n\nReason: {(e.Message != "" ? e.Message : "Unknown")}"));
        }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void ReportsButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.ReportsView);
        });

        await MainWindow.Instance.ReportsView.RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void ManageDBButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.DbView);
        });

        await MainWindow.Instance.DbView.RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void ChangePinButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        await ChangePinScreen.Instance.Show();
        UIDispatcher.UpdateUI();
    });

    private void LogoutButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        await LoginManager.Instance.Logout();
        UIDispatcher.UpdateUI();
    });

    private void UpsizeButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.ResizeSelectedItem(upsize: true); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void DownsizeButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.ResizeSelectedItem(upsize: false); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void NotImplementedButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
}
