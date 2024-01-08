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

public class MenuManager : ButtonView
{
    public OrderManager OrderManager { get; private set; }

    public string ViewName => "MenuView";

    public Menu? CurrentMenu { get; private set; }

    public List<Menu> RegisteredMenus { get; private set; } = new();

    public static MenuManager CreateTestMenus(MainWindow window)
    {
        // Burger Menu

        var burgerItem = new OrderListItem("Burger", 4.99M);
        var cheeseBurgerItem = burgerItem.NewFrom("Cheese Burger", 0.5M);
        var baconBurgerItem = burgerItem.NewFrom("Bacon Burger", 0.5M);
        var baconCheeseBurgerItem = burgerItem.NewFrom("Bacon Cheese Burger", 1M);

        var doubleBurgerItem = burgerItem.NewFrom("Dbl Burger", 1M);
        var doubleCheeseBurgerItem = doubleBurgerItem.NewFrom("Dbl Cheese Burger", 0.5M);
        var doubleBaconBurgerItem = doubleBurgerItem.NewFrom("Dbl Bacon Burger", 0.5M);
        var doubleBaconCheeseBurgerItem = doubleBurgerItem.NewFrom("Dbl Bacon Cheese Burger", 1M);

        var chickenBurgerItem = new OrderListItem("Chic Burger", 5.99M);
        var chickenCheeseBurgerItem = chickenBurgerItem.NewFrom("Chic Cheese Burger", 0.5M);
        var chickenBaconBurgerItem = chickenBurgerItem.NewFrom("Chic Bacon Burger", 0.5M);
        var chickenBaconCheeseBurgerItem = chickenBurgerItem.NewFrom("Chic Bacon Cheese Burger", 1M);

        var hawaiianBurgerItem = burgerItem.NewFrom("Hawaiian Burger", 2M);
        var tacoBurgerItem = burgerItem.NewFrom("Taco Burger", 1.5M);
        var sloppyBurgerItem = burgerItem.NewFrom("Sloppy Burger", 1.5M);
        var slawBurgerItem = burgerItem.NewFrom("Slaw Burger", 0.5M);

        MenuButton?[,] burgerMenuItems = {
            { new BasicMenuButton(burgerItem),              new BasicMenuButton(doubleBurgerItem),              new BasicMenuButton(chickenBurgerItem),             new BasicMenuButton(hawaiianBurgerItem) },
            { new BasicMenuButton(cheeseBurgerItem),        new BasicMenuButton(doubleCheeseBurgerItem),        new BasicMenuButton(chickenCheeseBurgerItem),       new BasicMenuButton(tacoBurgerItem) },
            { new BasicMenuButton(baconBurgerItem),         new BasicMenuButton(doubleBaconBurgerItem),         new BasicMenuButton(chickenBaconBurgerItem),        new BasicMenuButton(sloppyBurgerItem) },
            { new BasicMenuButton(baconCheeseBurgerItem),   new BasicMenuButton(doubleBaconCheeseBurgerItem),   new BasicMenuButton(chickenBaconCheeseBurgerItem),  new BasicMenuButton(slawBurgerItem) },
        };

        Menu burgerMenu = new Menu("Burgers", burgerMenuItems, 4, 4);
        MenuManager menuManager = new(window, window.OrderManager, burgerMenu);

        // Chicken Menu

        MenuButton?[,] chickenMenuItems = {
            { new BasicMenuButton(chickenBurgerItem),             new BasicMenuButton(hawaiianBurgerItem) },
            { new BasicMenuButton(chickenCheeseBurgerItem),       new BasicMenuButton(tacoBurgerItem) },
            { new BasicMenuButton(chickenBaconBurgerItem),        new BasicMenuButton(sloppyBurgerItem) },
            { new BasicMenuButton(chickenBaconCheeseBurgerItem),  new BasicMenuButton(slawBurgerItem) },
        };

        menuManager.CreateMenu("Chicken", chickenMenuItems, 4, 4);

        return menuManager;
    }

    public MenuManager(MainWindow window, OrderManager orderManager, Menu? startingMenu = null) : base(window)
    {
        OrderManager = orderManager;
        CurrentMenu = startingMenu;

        if (startingMenu != null) RegisterMenu(startingMenu);
    }

    public Menu CreateMenu(string name, MenuButton?[,] items, int? rows = null, int? columns = null)
    {
        Menu menu = new Menu(name, items, rows, columns);
        RegisterMenu(menu);

        return menu;
    }

    public void RegisterMenu(Menu menu)
    {
        if (RegisteredMenus.Contains(menu)) return;
        RegisteredMenus.Add(menu);

        Button button = new();
        button.Content = menu.Name;
        button.Click += (_, _) => ShowMenu(menu);

        Window.SP_MenuList.Children.Add(button);
    }

    public override void OnSetMainView()
    {
        base.OnSetMainView();
        if (CurrentMenu != null) ShowMenu(CurrentMenu);
    }

    public void ShowMenu(Menu? menu)
    {
        Window.MainViewManager.ShowView(ViewName);

        Buttons.ForEach(b => Grid!.Children.Remove(b));
        CurrentMenu = menu;

        Rows = menu?.Rows ?? 0;
        Columns = menu?.Columns ?? 0;

        if (menu == null) return;

        for (int row = 0; row < menu.Items.GetLength(0); row++)
        {
            for (int column = 0; column < menu.Items.GetLength(1); column++)
            {
                var item = menu.Items[row, column];
                if (item == null) continue;

                SetItemButton(row, column, item);
            }
        }
    }

    void SetItemButton(int row, int column, MenuButton item)
    {
        if (CurrentMenu == null) throw new ArgumentNullException();

        SetButton(item.CreateButton(this), row, column);
    }
}
