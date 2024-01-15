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


public class BasicMenuButton : MenuButton
{
    public OrderListItem Item { get; protected set; }

    public BasicMenuButton(OrderListItem item, string? displayText = null)
        : base(displayText ?? item.Text) {
        Item = item;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MenuView menu)
    {
        var view = menu.OrderManager.AddOrder(Item);
        menu.OrderManager.SelectItem(view);
    }
}
