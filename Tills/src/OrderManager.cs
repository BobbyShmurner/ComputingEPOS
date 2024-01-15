using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;

public enum CheckoutType
{
    SitIn,
    TakeAway
}

public static class CheckoutTypeExtensions
{
    public static string ToPrettyString(this CheckoutType checkoutType)
    {
        switch (checkoutType)
        {
            case CheckoutType.SitIn: return "Sit In";
            case CheckoutType.TakeAway: return "Take Away";
            default: return checkoutType.ToString();
        }
    }
}

public class OrderManager : INotifyPropertyChanged {
    

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<OrderListItemView?>? OnSelectionChanged;

    public MenuView Menu { get; private set; }
    public OrderMenuManager OrderMenuManager => Menu.OrderMenuManager;

    public List<OrderListItemView> RootItems { get; private set; } = [];
    public OrderListItemView? Selected { get; private set; }
    public Dictionary<OrderListItem, OrderListItemView> Views = [];

    bool m_IsItemSelected = false;
    public bool IsItemSelected {
        get => m_IsItemSelected;
        private set
        {
            m_IsItemSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsItemSelected)));
        }
    }

    bool m_IsOrderLocked = false;
    public bool IsOrderLocked
    {
        get => m_IsOrderLocked;
        private set
        {
            m_IsOrderLocked = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOrderLocked)));
        }
    }

    decimal m_Total = 0;
    public decimal Total {
        get => m_Total;
        set
        {
            m_Total = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubTotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tax)));
        }
    }

    int m_OrderNumber = 1;
    public int OrderNumber {
        get => m_OrderNumber;
        set
        {
            m_OrderNumber = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderNumber)));
        }
    }

    CheckoutType? m_CheckoutType;
    public CheckoutType? CheckoutType
    {
        get => m_CheckoutType;
        set
        {
            m_CheckoutType = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckoutType)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckoutTypePretty)));
        }
    }

    public string CheckoutTypePretty => CheckoutType?.ToPrettyString() ?? "None";

    public decimal SubTotal => Total * 0.8M;
    public decimal Tax => Total * 0.2M;
    public decimal Outstanding => 0M;

    public OrderManager(MenuView menu)
    {
        Menu = menu;

        OnSelectionChanged += (itemView) => IsItemSelected = itemView != null;
        OrderMenuManager.OnMenuChanged += menu => IsOrderLocked = menu != null;
    }

    public OrderListItemView AddOrder(OrderListItem item, OrderListItemView? parent = null)
    {
        var view = new OrderListItemView(this, Menu, item, parent);
        Views[item] = view;

        if (item.Price.HasValue) Total += item.Price.Value;
        if (parent == null) RootItems.Add(view);

        return view;
    }

    public OrderListItemView? GetNextOrderForSelection(OrderListItemView view)
    {
        if (view.Parent != null) return view.Parent;
        int index = RootItems.IndexOf(view);

        if (index < RootItems.Count - 1) return RootItems[index + 1];
        if (index > 0) return RootItems[index - 1];

        return null;
    }

    public void RemoveSelectedOrder() {
        if (Selected == null) return;
        OrderListItemView? nextSelection = GetNextOrderForSelection(Selected);

        RemoveOrder(Selected);
        SelectItem(nextSelection);
    }

    public void RemoveOrder(OrderListItemView view)
    {
        if (view.Parent == null) RootItems.Remove(view);
        if (view.Price.HasValue) Total -= view.Price.Value;

        view.Remove();
    }

    public void DeleteAll()
    {
        while (RootItems.Count > 0) RemoveOrder(RootItems[0]);
    }

    public void DeselectItem(bool fireEvent = true)
    {
        RootItems.ForEach(item => item.RecursivlyHideBorder());
        Selected = null;

        if (fireEvent) OnSelectionChanged?.Invoke(null);
    }

    public void CheckoutOrder(CheckoutType checkoutType) {
        CheckoutType = checkoutType;
        LockOrder();
        OrderMenuManager.ShowPaymentScreen();
    }

    public void LockOrder()
    {
        IsOrderLocked = true;
        DeselectItem();
    }

    public void UnlockOrder()
    {
        IsOrderLocked = false;
        SelectLastItem();
    }

    public void SelectItem(OrderListItemView? view)
    {
        DeselectItem(false);
        if (view == null) return;
        if (IsOrderLocked) return;

        view.Selected = true;
        Selected = view;

        OnSelectionChanged?.Invoke(Selected);
    }

    public void SelectLastItem() => SelectItem(RootItems.LastOrDefault());
}
