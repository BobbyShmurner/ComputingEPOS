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

public class MenuManager {
    public MainWindow Window { get; private set; }
    public Grid Grid => Window.Grid_MenuItems;
    public OrderManager OrderManager { get; private set; }

    public Menu? CurrentMenu { get; private set; }

    public List<Menu> RegisteredMenus { get; private set; } = new();

    public List<Button> Buttons { get; private set; } = new();

    int m_Rows;
    public int Rows
    {
        get => m_Rows;
        set
        {
            m_Rows = value;

            var RowDefinitions = Grid.RowDefinitions;
            RowDefinitions.Clear();

            for (int i = 0; i < m_Rows; i++)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
    }

    int m_Columns;
    public int Columns
    {
        get => m_Columns;
        set
        {
            m_Columns = value;

            var ColumnDefinitions = Grid.ColumnDefinitions;
            ColumnDefinitions.Clear();

            for (int i = 0; i < m_Columns; i++)
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    }

    public MenuManager(MainWindow window, OrderManager orderManager, Menu? startingMenu = null)
    {
        Window = window;
        OrderManager = orderManager;

        if (startingMenu != null)
        {
            RegisterMenu(startingMenu);
            SetMenu(startingMenu);
        }
    }

    public Menu CreateMenu(string name, MenuButton?[,] items)
    {
        Menu menu = new Menu(name, items);
        RegisterMenu(menu);

        return menu;
    }

    public void RegisterMenu(Menu menu) 
    {
        if (RegisteredMenus.Contains(menu)) return;
        RegisteredMenus.Add(menu);

        Button button = new();
        button.Content = menu.Name;
        button.Click += (_, _) => SetMenu(menu);

        Window.SP_MenuList.Children.Add(button);
    }

    public void SetMenu(Menu? menu)
    {
        Buttons.ForEach(b => Grid.Children.Remove(b));
        CurrentMenu = menu;

        Rows = menu?.Rows ?? 0;
        Columns = menu?.Columns ?? 0;

        if (menu == null) return;

        for (int row = 0; row < menu.Items.GetLength(0); row++) {
            for (int column = 0; column < menu.Items.GetLength(1); column++) {
                var item = menu.Items[row, column];
                if (item == null) continue;

                SetItemButton(row, column, item);
            }
        }
    }

    void SetItemButton(int row, int column, MenuButton item)
    {
        if (row >= Rows) throw new IndexOutOfRangeException();
        if (column >= Columns) throw new IndexOutOfRangeException();
        if (CurrentMenu == null) throw new ArgumentNullException();

        var button = item.CreateButton(this);
        Buttons.Add(button);

        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);

        Window.Grid_MenuItems.Children.Add(button);
    }
}
