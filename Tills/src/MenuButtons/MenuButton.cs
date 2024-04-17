using ComputingEPOS.Common;
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

public abstract class MenuButton
{
    public string DisplayText { get; protected set; }
    public Button? button { get; protected set; }

    public MenuButton(string displayText) {
        DisplayText = displayText;
    }

    public Button CreateButton(MenuView menu) {
        button = new();

        TextBlock textBlock = new TextBlock {
            Text = DisplayText
        };

        button.Content = textBlock;
        button.Click += (sender, e) => OnClick(sender, e, menu);

        button = PostCreateButton(button, menu);
        return button;
    }

    protected virtual Button PostCreateButton(Button button, MenuView menu) => button;

    protected abstract void OnClick(object sender, RoutedEventArgs e, MenuView menu);
}
