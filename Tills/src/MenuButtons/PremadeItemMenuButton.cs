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


public class PremadeItemMenuButton : MenuButton
{
    public OrderListItem Item { get; protected set; }

    public PremadeItemMenuButton(OrderListItem item, string? displayText = null)
        : base(displayText ?? item.Text) {
        Item = item;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MenuView menu)
    {
        UIDispatcher.EnqueueUIAction(async () => {
            var view = await menu.OrderManager.AddOrderItem(Item);
            await UIDispatcher.UpdateUIAsync();
        });
    }
}
