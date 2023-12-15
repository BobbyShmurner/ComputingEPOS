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
    public string DisplayText { get; protected set; }
    public OrderListItem Item { get; protected set; }
    public Button? button { get; protected set; }

    public BasicMenuButton(OrderListItem item, string? displayText = null)
    {
        Item = item;
        DisplayText = displayText ?? Item.Text;
    }

    public override Button CreateButton(MenuManager menuManager)
    {
        button = new();

        button.Content = DisplayText;
        button.Click += (_, _) =>
        {
            var view = menuManager.OrderManager.AddOrder(Item);
            menuManager.OrderManager.SelectItem(view);
        };

        return button;
    }
}
