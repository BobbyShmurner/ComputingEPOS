using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;

public class OrderManager
{
    public MainWindow Window { get; private set; }

    public List<OrderListItemView> RootItems { get; private set; } = new();
    public Dictionary<OrderListItem, OrderListItemView> Views = new();

    OrderListItemView? m_Selected = null;
    public OrderListItemView? Selected
    {
        get => m_Selected;
        private set
        {
            m_Selected = value;
            Window.EnableButtonsWhenItemSelected = m_Selected != null;
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
    }

    public OrderListItemView AddOrder(OrderListItem item, OrderListItemView? parent = null)
    {
        var view = new OrderListItemView(this, Window, item, parent != null ? parent : null);
        Views[item] = view;

        if (item.Price.HasValue) Total += item.Price.Value;
        if (parent == null) RootItems.Add(view);

        SelectItem(view);
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

    public void DeselectItem()
    {
        RootItems.ForEach(item => item.RecursivlyHideBorder());
        Selected = null;
    }

    public void SelectItem(OrderListItemView? view)
    {
        DeselectItem();
        if (view == null) return;

        view.Selected = true;
        Selected = view;
    }
}
