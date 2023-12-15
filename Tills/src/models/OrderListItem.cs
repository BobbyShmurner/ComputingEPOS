﻿using System;
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
    // Text cannot be null, so if m_Text is null, the base item cannot be.
    string? m_Text;
    public string Text => m_Text ?? BaseItem!.Text;

    // Price can actually be null, so there might not be a base item.
    decimal? m_Price;
    public decimal? Price => m_Price ?? BaseItem?.Price; 

    public OrderListItem? BaseItem { get; private set; }
    public List<OrderListItem> Children { get; private set; }

    public OrderListItem(OrderListItem baseItem, params OrderListItem[] children) {
        BaseItem = baseItem;
        Children = children.ToList();
    }

    public OrderListItem(string text, decimal? price = null, params OrderListItem[] children)
    {
        m_Text = text;
        m_Price = price;
        Children = children.ToList();
    }

    public OrderListItem NewFrom(params OrderListItem[] children) => new(this, children);
}
