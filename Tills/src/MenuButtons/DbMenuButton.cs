using ComputingEPOS.Tills.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;


public class DbMenuButton : MenuButton
{
    public OrderListItem Item { get; protected set; }
    public int StockID { get; protected set; }

    public DbMenuButton(OrderListItem item, int stockID, string? displayText = null)
        : base(displayText ?? item.Text) {
        Item = item;
        StockID = stockID;
    }

    protected override void OnClick(object sender, RoutedEventArgs e, MenuView menu)
    {
        // Task.Run(async () => {
        //     while (menu.OrderManager.CurrentOrder == null) Thread.Sleep(100);
        //     var returnedOrderItem = await OrderItems.Create(menu.OrderManager.CurrentOrder!.OrderID, StockID, 1, Item.Price);
        //     var itemClone = Item.Clone();
        // 
        //     itemClone.OrderItem = returnedOrderItem;
        // 
        //     var view = await menu.OrderManager.AddOrderItem(itemClone);
        //     menu.OrderManager.Menu.Dispatcher.Invoke(() => {
        //         menu.OrderManager.SelectItem(view);
        //     });
        // });
    }
}
