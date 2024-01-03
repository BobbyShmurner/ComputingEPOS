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

    public Button CreateButton(MenuManager menuManager) {
        button = new();

        TextBlock textBlock = new TextBlock();
        textBlock.Text = DisplayText;
        textBlock.TextWrapping = TextWrapping.Wrap;
        textBlock.TextAlignment = TextAlignment.Center;

        button.Content = textBlock;
        button.Click += (sender, e) => OnClick(sender, e, menuManager);

        button = PostCreateButton(button, menuManager);
        return button;
    }

    protected virtual Button PostCreateButton(Button button, MenuManager menuManager) => button;

    protected abstract void OnClick(object sender, RoutedEventArgs e, MenuManager menuManager);
}
