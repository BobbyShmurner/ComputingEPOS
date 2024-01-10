using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;


public class SubItemMenuButton : BasicMenuButton
{

    public SubItemMenuButton(OrderListItem item, string? displayText = null) : base(item, displayText) { }

    protected override Button PostCreateButton(Button button, MainWindow window) {
        window.OrderManager.OnSelectionChanged += OnSelectionChanged;
        OnSelectionChanged(window.OrderManager.Selected);

        return button;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MainWindow window) {
        var parent = window.OrderManager.Selected?.RootParent ?? window.OrderManager.Selected;
        window.OrderManager.AddOrder(Item, parent);
        window.OrderManager.SelectItem(parent);
    }

    void OnSelectionChanged(OrderListItemView? selection) => button!.IsEnabled = selection != null;
}
