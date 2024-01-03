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

    protected override Button PostCreateButton(Button button, MenuManager menuManager) {
        menuManager.OrderManager.OnSelectionChanged += OnSelectionChanged;
        OnSelectionChanged(menuManager.OrderManager.Selected);

        return button;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MenuManager menuManager) {
        var parent = menuManager.OrderManager.Selected?.RootParent ?? menuManager.OrderManager.Selected;
        menuManager.OrderManager.AddOrder(Item, parent);
        menuManager.OrderManager.SelectItem(parent);
    }

    void OnSelectionChanged(OrderListItemView? selection) => button!.IsEnabled = selection != null;
}
