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

using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Tills;


public class OrderListItem {
    // Text cannot be null, so if m_Text is null, the base item cannot be.
    string? m_Text;
    public string Text => m_Text ?? BaseItem!.Text;

    // Price can actually be null, so there might not be a base item.
    decimal? m_Price;
    public decimal? Price => m_Price ?? BaseItem?.Price;

    public int? StockID;

    public OrderItem? OrderItem = null;

    public OrderListItem? BaseItem { get; private set; }
    public List<OrderListItem> Children { get; private set; }

    OrderListItem() {
        Children = new();
    }

    OrderListItem(OrderListItem baseItem, int? stockID, params OrderListItem[] children) {
        BaseItem = baseItem;
        Children = children.ToList();
        StockID = stockID;
    }

    public OrderListItem(string text, params OrderListItem[] children)
    {
        m_Text = text;
        Children = children.ToList();
    }

    public OrderListItem(string text, int stockID, decimal? price = null, params OrderListItem[] children)
    {
        m_Text = text;
        m_Price = price;
        Children = children.ToList();
        StockID = stockID;
    }

    public OrderListItem NewWithChildren(int? stockID, params OrderListItem[] children) => new(this, stockID, children);
    public OrderListItem NewFrom(string text, int stockID, decimal priceDelta) => new(text, stockID, price: (Price ?? 0) + priceDelta, Children.ToArray());

    public OrderListItem Clone() => new OrderListItem
    {
        m_Text = m_Text,
        m_Price = m_Price,
        BaseItem = BaseItem?.Clone(),
        Children = Children.Select(c => c.Clone()).ToList(),
        OrderItem = OrderItem,
        StockID = StockID,
    };

    public void RenameText(string newText) => m_Text = newText;
}
