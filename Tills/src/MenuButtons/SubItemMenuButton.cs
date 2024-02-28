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


public class SubItemMenuButton : PremadeItemMenuButton {
    public SubItemMenuButton(OrderListItem item, string? displayText = null) : base(item, displayText) { }

    protected override Button PostCreateButton(Button button, MenuView menu) {
        menu.OrderManager.OnSelectionChanged += OnSelectionChanged;
        OnSelectionChanged(menu.OrderManager.Selected);

        return button;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MenuView menu) {
        var parent = menu.OrderManager.Selected?.RootParent ?? menu.OrderManager.Selected;
        
        UIDispatcher.EnqueueUIAction(async () => {
            var view = await menu.OrderManager.AddOrderItem(Item, parent);
            menu.OrderManager.SelectItem(parent);
            UIDispatcher.UpdateUI();
        });
    }

    void OnSelectionChanged(OrderListItemView? selection) => button!.IsEnabled = selection != null;
}
