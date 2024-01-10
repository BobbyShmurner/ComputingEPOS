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

    public Button CreateButton(MainWindow window) {
        button = new();

        TextBlock textBlock = new TextBlock();
        textBlock.Text = DisplayText;

        button.Content = textBlock;
        button.Click += (sender, e) => OnClick(sender, e, window);

        button = PostCreateButton(button, window);
        return button;
    }

    protected virtual Button PostCreateButton(Button button, MainWindow window) => button;

    protected abstract void OnClick(object sender, RoutedEventArgs e, MainWindow window);
}
