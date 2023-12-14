using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;

public class OrderListItemView {
    const int INDENT_AMOUNT = 10;
    const int BASE_INDENT = 5;
    const int PRICE_GAP = 5;

    public MainWindow Window { get; private set; }
    public List<OrderListItemView> Children { get; private set; } = new();
    public OrderListItemView? Parent { get; private set; }

    public OrderListItem Item { get; private set; }
    public OrderManager Manager { get; private set; }

    string m_Text;
    public string Text
    {
        get => m_Text;
        set
        {
            m_Text = value;
            textBlock.Text = $"- {m_Text}";
        }
    }

    decimal? m_Price = null;
    public decimal? Price { 
        get => m_Price;
        set {
            m_Price = value;
            priceText.Text = m_Price != null ? $"£{m_Price:0.00}" : "";
        }
    }

    public int Indent => Parent != null ? Parent.Indent + INDENT_AMOUNT : BASE_INDENT;

    bool m_Selected = false;
    public bool Selected
    {
        get => m_Selected;
        set
        {
            m_Selected = value;
            if (m_Selected)
            {
                border.Background = Brushes.LightGray;
                border.BorderThickness = new Thickness(0, 1, 0, 1);
                stackPanel.Margin = new Thickness(0, -1, 0, -1);
            } else
            {
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(0);
                stackPanel.Margin = new Thickness(0);
            }
        }
    }

    DockPanel dockPanel;
    StackPanel stackPanel;
    TextBlock textBlock;
    TextBlock priceText;
    Border border;

    public OrderListItemView(OrderManager manager, MainWindow window, OrderListItem item, OrderListItemView? parent = null) {
        Window = window;
        Parent = parent;
        Manager = manager;

        Item = item;

        if (Parent != null) Parent.Children.Add(this);

        border = new Border();
        border.BorderBrush = Brushes.DimGray;
        DockPanel.SetDock(border, Dock.Top);

        stackPanel = new StackPanel();
        border.Child = stackPanel;

        dockPanel = new DockPanel();
        dockPanel.MouseDown += OnMouseDown;
        stackPanel.Children.Add(dockPanel);

        priceText = new TextBlock();
        DockPanel.SetDock(priceText, Dock.Right);
        priceText.Padding = new Thickness(PRICE_GAP, 0, BASE_INDENT, 0);
        dockPanel.Children.Add(priceText);

        textBlock = new TextBlock();
        textBlock.Padding = new Thickness(Indent, 0, BASE_INDENT, 0);
        dockPanel.Children.Add(this.textBlock);

        m_Text = Item.Text; // Redundant, just gets rid of a warning
        Text = Item.Text;

        Price = Item.Price;

        if (Parent != null) Parent.stackPanel.Children.Add(border);
        else Window.DP_OrderItems.Children.Insert(Window.DP_OrderItems.Children.Count - 1, border);

        Item.Children.ForEach(c => Manager.AddOrder(c, this));
        border.BringIntoView();
    }

    void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Trace.WriteLine(Text);
        if (!Selected) Manager.SelectItem(this);
        else Manager.DeselectItem();
    }

    public void Remove() {
        while (Children.Count > 0)
        {
            Manager.RemoveOrder(Children[0]);
        }

        if (Manager.Selected == this) Manager.DeselectItem();

        if (Parent != null)
        {
            Parent.Children.Remove(this);
            Parent.stackPanel.Children.Remove(border);
        }
        else
        {
            Window.DP_OrderItems.Children.Remove(border);
        }
    }

    public void RecursivlyHideBorder()
    {
        Selected = false;
        Children.ForEach(c => c.RecursivlyHideBorder());
    }

    public void BringIntoView() => border.BringIntoView();
}
