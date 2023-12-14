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

public class OrderListItem {
    public string Text { get; private set; }
    public decimal? Price { get; private set; }
    public List<OrderListItem> Children { get; private set; }

    public OrderListItem(string text, decimal? price = null, List<OrderListItem>? children = null)
    {
        Text = text;
        Price = price;
        Children = children ?? new();
    }
}
