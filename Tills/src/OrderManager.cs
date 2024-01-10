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

public class OrderManager : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<OrderListItemView?>? OnSelectionChanged;

    public MainWindow Window { get; private set; }
    public OrderMenuManager OrderMenuManager => Window.OrderMenuManager;

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
            Window.Text_Total.Text = $"£{m_Total:0.00}";
        }
    }

    public OrderManager(MainWindow window)
    {
        Window = window;

        OnSelectionChanged += (itemView) => IsItemSelected = itemView != null;
        OrderMenuManager.OnMenuChanged += menu => IsOrderLocked = menu != null;
    }

    public OrderListItemView AddOrder(OrderListItem item, OrderListItemView? parent = null)
    {
        var view = new OrderListItemView(this, Window, item, parent);
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

    public void PayForOrder() {
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
