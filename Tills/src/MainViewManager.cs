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
using System.Xml.Linq;

namespace ComputingEPOS.Tills;

public class MainViewManager
{
    public MainWindow Window { get; private set; }
    public Border Border => Window.Border_MainView;

    public IMainView? CurrentView { get; private set; }
    public Dictionary<string, IMainView> RegisteredViews { get; private set; } = new();

    public MainViewManager(MainWindow window)
    {
        Window = window;
    }

    public void RegisterView(IMainView view, string viewName) 
    {
        if (RegisteredViews.ContainsKey(viewName)) return;
        RegisteredViews.Add(viewName, view);
    }

    public void ShowView(string viewName)
    {
        var newView = RegisteredViews[viewName];
        if (newView == CurrentView) return;

        CurrentView = newView;
        CurrentView.OnSetMainView();
        Border.Child = CurrentView.ViewElement;
    }
}
