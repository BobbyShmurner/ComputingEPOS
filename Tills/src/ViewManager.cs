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

public class ViewManager
{
    public MainWindow Window { get; private set; }
    public Grid Root { get; private set; }
    public FrameworkElement? CurrentView { get; private set; }

    public event Action<FrameworkElement?, FrameworkElement?>? OnViewChanged;

    public ViewManager(MainWindow window, Grid rootElement)
    {
        Window = window;
        Root = rootElement;

        Init();
    }

    void Init() {
        foreach (FrameworkElement child in Root.Children) {
            if (CurrentView == null)
            {
                if (child.Visibility != Visibility.Visible) continue;
                CurrentView = child;
            } else
            {
                child.Visibility = Visibility.Collapsed;
            }
        }
    }

    public void HideView() {
        if (CurrentView == null) return;
        CurrentView.Visibility = Visibility.Collapsed;
        CurrentView = null;
    }

    public void ShowView(FrameworkElement? view) {
        if (view == null) {
            HideView();
            return;
        }

        ShowView(view.Name);
    }
    public void ShowView(string viewName) {
        viewName = viewName.ToLower().Trim();
        var oldView = CurrentView;

        if (CurrentView != null) {
            if (viewName == CurrentView.Name.ToLower().Trim()) return;
            CurrentView.Visibility = Visibility.Collapsed;
        }

        foreach (FrameworkElement child in Root.Children) {
            child.Visibility = child.Name.ToLower().Trim() == viewName ? Visibility.Visible : Visibility.Collapsed;
            if (child.Visibility == Visibility.Visible) {
                Trace.WriteLine("Found it!");
                CurrentView = child;
                OnViewChanged?.Invoke(CurrentView, oldView);

                return;
            }
        }

        CurrentView = null;
        OnViewChanged?.Invoke(CurrentView, oldView);
    }
}
