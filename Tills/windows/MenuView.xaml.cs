﻿using ComputingEPOS.Tills.Api;
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

namespace ComputingEPOS.Tills;

public partial class MenuView : UserControl
{
    public const int SCROLL_AMOUNT = 100;

    public OrderManager OrderManager { get; private set; }
    public OrderMenuManager OrderMenuManager { get; private set; }

    public MenuView()
    {
        InitializeComponent();
        this.DataContext = this;

        OrderMenuManager = OrderMenuManager.CreateTestMenus(this);
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

    private void Orders_SV_Up(object sender, RoutedEventArgs e) => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset - SCROLL_AMOUNT);
    private void Orders_SV_Down(object sender, RoutedEventArgs e) => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset + SCROLL_AMOUNT);

    private void Button_Clear(object sender, RoutedEventArgs e) => Task.Run(async() => await OrderManager.DeleteAllItems(true));

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => OrderManager.RemoveSelectedOrderItem(true);
    private void ModifyButton_Click(object sender, RoutedEventArgs e) { }

    private void SitInButton_Click(object sender, RoutedEventArgs e) => OrderManager.CheckoutOrder(CheckoutType.SitIn);
    private void TakeAwayButton_Click(object sender, RoutedEventArgs e) => OrderManager.CheckoutOrder(CheckoutType.TakeAway);

    private void FunctionsButton_Click(object sender, RoutedEventArgs e) { }

    private void OrderItemsEmptyFillButton_Click(object sender, RoutedEventArgs e) => OrderManager.DeselectItem();

    private void TransactionButton_Click(object sender, RoutedEventArgs e) => OrderManager.PayForOrder(TransactionButton.GetAmount((UIElement)sender), TransactionButton.GetPaymentMethod((UIElement)sender), TransactionButton.GetSpecial((UIElement)sender));

    private void ComboButton_Click(object sender, RoutedEventArgs e) => OrderManager.MakeSelectedCombo();
}
