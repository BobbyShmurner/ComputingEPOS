﻿using System;
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


public class EbbMenuButton : MenuItemMenuButton
{
    public double FontSize { get; private set; }

    public EbbMenuButton(OrderListItem item, string? displayText = null, double fontSize = 16)
        : base(item, displayText) {
        FontSize = fontSize;
    }

    protected override Button PostCreateButton(Button button, MenuView menu)
    {
        button = base.PostCreateButton(button, menu);
        TextBlock text = (TextBlock)button.Content;
        text.FontFamily = new FontFamily("Comic Sans MS");
        text.FontSize = FontSize;

        return button;
    }
}
