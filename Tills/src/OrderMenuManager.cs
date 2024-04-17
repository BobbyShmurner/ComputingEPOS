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
using Models = ComputingEPOS.Common.Models;
namespace ComputingEPOS.Tills;

public class OrderMenuManager {
    public Action<Menu?>? OnMenuChanged;
    public Action? OnShowPaymentScreen;
    public Action? OnMenusLoaded;

    public MenuView Menu { get; private set; }
    public Grid Root => Menu.Grid_MenuButtons;
    public Grid MenuView => Menu.Grid_MenuButtonsOrderView;
    public Grid PaymentView => Menu.Grid_MenuButtonsPaymentView;
    public Grid FunctionsView => Menu.Grid_MenuButtonsFunctionsView;
    public Grid KeypadView => Menu.Grid_MenuButtonsKeypadView;
    public OrderManager OrderManager => Menu.OrderManager;
    public ViewManager MenuViewManager { get; private set; }

    public Menu? CurrentMenu { get; private set; }

    public List<Menu> RegisteredMenus { get; private set; } = [];

    public List<Button> Buttons { get; private set; } = [];

    public bool LoadingMenus { get; private set; }

    int m_Rows;
    public int Rows {
        get => m_Rows;
        set {
            m_Rows = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                var RowDefinitions = MenuView.RowDefinitions;
                RowDefinitions.Clear();

                for (int i = 0; i < m_Rows; i++)
                    RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            });
        }
    }

    int m_Columns;
    public int Columns {
        get => m_Columns;
        set {
            m_Columns = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                var ColumnDefinitions = MenuView.ColumnDefinitions;
                ColumnDefinitions.Clear();

                for (int i = 0; i < m_Columns; i++)
                    ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            });
        }
    }

    public async Task RefreshMenusFromDB() {
        LoadingMenus = true;

        List<Models.Menu> menuModels = await Api.Menus.GetMenus(visible: true);
        List<Models.MenuItem> menuItemModels = await Api.MenuItems.GetMenuItems();
        List<Models.Menu_MenuItem> menu_MenuItemModels = await Api.Menu_MenuItems.GetMenu_MenuItems();
        List<Models.Stock> stockModels = await Api.Stock.GetStock();

        UIDispatcher.EnqueueOnUIThread(() => {
            Dictionary<int, OrderListItem> listItems = new();
            UnregisterAllMenus();

            foreach (var menuItemModel in menuItemModels) {
                Models.Stock? stock = stockModels.FirstOrDefault(s => s.StockID == menuItemModel.StockID);
                if (stock == null) continue;

                listItems[menuItemModel.MenuItemID] = new OrderListItem(stock?.Name ?? "[UNKNOWN]", stock!.StockID, menuItemModel.Price);
            }

            foreach (var menuModel in menuModels) {
                var joinModels = menu_MenuItemModels.Where(mmi => mmi.MenuID == menuModel.MenuID).ToList();

                int rows = joinModels.Count > 0 ? joinModels.Max(mmi => mmi.Row) + 1 : 0;
                int columns = joinModels.Count > 0 ? joinModels.Max(mmi => mmi.Column) + 1 : 0;

                MenuButton[,] menuItems = new MenuButton[rows, columns];

                foreach (var joinModel in joinModels) {
                    if (!listItems.ContainsKey(joinModel.MenuItemID)) continue;
                    menuItems[joinModel.Row, joinModel.Column] = new MenuItemMenuButton(listItems[joinModel.MenuItemID]);
                }

                Menu menu = new(menuModel.Name, menuItems, menuModel.Rows, menuModel.Columns);
                RegisterMenu(menu);
            }

            LoadingMenus = false;
            OnMenusLoaded?.Invoke();
        });
    }

    public static OrderMenuManager CreateTestMenus(MenuView menu)
    {
        // Burger Menu

        var burgerItem = new OrderListItem("Burger", 0, 4.99M);
        var cheeseBurgerItem = burgerItem.NewFrom("Cheese Burger", 2, 0.5M);
        var baconBurgerItem = burgerItem.NewFrom("Bacon Burger", 4, 0.5M);
        var baconCheeseBurgerItem = burgerItem.NewFrom("Bacon Cheese Burger", 7, 1M);

        var doubleBurgerItem = burgerItem.NewFrom("Dbl Burger", 1, 1M);
        var doubleCheeseBurgerItem = doubleBurgerItem.NewFrom("Dbl Cheese Burger", 3, 0.5M);
        var doubleBaconBurgerItem = doubleBurgerItem.NewFrom("Dbl Bacon Burger", 6, 0.5M);
        var doubleBaconCheeseBurgerItem = doubleBurgerItem.NewFrom("Dbl Bacon Cheese Burger", 8, 1M);

        var chickenBurgerItem = new OrderListItem("Chick Burger", 9, 5.99M);
        var chickenCheeseBurgerItem = chickenBurgerItem.NewFrom("Chick Cheese Burger", 10, 0.5M);
        var chickenBaconBurgerItem = chickenBurgerItem.NewFrom("Chick Bacon Burger", 11, 0.5M);
        var chickenBaconCheeseBurgerItem = chickenBurgerItem.NewFrom("Chick Bacon Cheese Burger", 12, 1M);

        var hawaiianBurgerItem = burgerItem.NewFrom("Hawaiian Burger", 13, 2M);
        var tacoBurgerItem = burgerItem.NewFrom("Taco Burger", 14, 1.5M);
        var sloppyBurgerItem = burgerItem.NewFrom("Sloppy Burger", 15, 1.5M);
        var slawBurgerItem = burgerItem.NewFrom("Slaw Burger", 16, 0.5M);

        var chickenNuggets4 = new OrderListItem("4 Chick Nuggs", 24, 3.99M);
        var chickenNuggets6 = chickenNuggets4.NewFrom("6 Chick Nuggs", 25, 1M);
        var chickenNuggets9 = chickenNuggets6.NewFrom("9 Chick Nuggs", 26, 1M);
        var chickenNuggets20 = chickenNuggets9.NewFrom("20 Chick Nuggs", 27, 2M);

        MenuButton[,] burgerMenuItems = {
            { new MenuItemMenuButton(burgerItem),              new MenuItemMenuButton(doubleBurgerItem),              new MenuItemMenuButton(chickenBurgerItem),             new MenuItemMenuButton(hawaiianBurgerItem) },
            { new MenuItemMenuButton(cheeseBurgerItem),        new MenuItemMenuButton(doubleCheeseBurgerItem),        new MenuItemMenuButton(chickenCheeseBurgerItem),       new MenuItemMenuButton(tacoBurgerItem) },
            { new MenuItemMenuButton(baconBurgerItem),         new MenuItemMenuButton(doubleBaconBurgerItem),         new MenuItemMenuButton(chickenBaconBurgerItem),        new MenuItemMenuButton(sloppyBurgerItem) },
            { new MenuItemMenuButton(baconCheeseBurgerItem),   new MenuItemMenuButton(doubleBaconCheeseBurgerItem),   new MenuItemMenuButton(chickenBaconCheeseBurgerItem),  new MenuItemMenuButton(slawBurgerItem) },
        };

        Menu burgerMenu = new("Burgers", burgerMenuItems, 4, 4);
        OrderMenuManager menuManager = new(menu, burgerMenu);

        // Chicken Menu

        MenuButton[,] chickenMenuItems = {
            { new MenuItemMenuButton(chickenBurgerItem),             new MenuItemMenuButton(chickenNuggets4), },
            { new MenuItemMenuButton(chickenCheeseBurgerItem),       new MenuItemMenuButton(chickenNuggets6), },
            { new MenuItemMenuButton(chickenBaconBurgerItem),        new MenuItemMenuButton(chickenNuggets9), },
            { new MenuItemMenuButton(chickenBaconCheeseBurgerItem),  new MenuItemMenuButton(chickenNuggets20), },
        };

        menuManager.CreateMenu("Chicken", chickenMenuItems, 4, 4);

        // Drinks Menu

        MenuButton[,] drinkMenuItems = {
            { new MenuItemMenuButton(chickenBurgerItem),            },
            { new MenuItemMenuButton(chickenCheeseBurgerItem),      },
            { new MenuItemMenuButton(chickenBaconBurgerItem),       },
            { new MenuItemMenuButton(chickenBaconCheeseBurgerItem), },
        };

        menuManager.CreateMenu("Drinks", drinkMenuItems, 4, 4);

        // Sides Menu

        MenuButton[,] sidesMenuItems = {
            { new MenuItemMenuButton(chickenBurgerItem),            },
            { new MenuItemMenuButton(chickenCheeseBurgerItem),      },
            { new MenuItemMenuButton(chickenBaconBurgerItem),       },
            { new MenuItemMenuButton(chickenBaconCheeseBurgerItem), },
        };

        menuManager.CreateMenu("Sides", sidesMenuItems, 4, 4);

        // eeb

        // var glutenItem = new OrderListItem("Gluten Free", 1000000M);
        // var burbgereItem = new OrderListItem("burbgere", 5.99M, [glutenItem]);

        // MenuButton?[,] eebMenuItems = {
        //     { new EbbMenuButton(burbgereItem, fontSize: 75),             },
        // };

        // menuManager.CreateMenu("menu but nyo french", eebMenuItems);

        return menuManager;
    }

    public OrderMenuManager(MenuView menu, Menu? startingMenu = null)
    {
        Menu = menu;
        CurrentMenu = startingMenu;
        MenuViewManager = new(Root);

        if (startingMenu != null)
        {
            RegisterMenu(startingMenu);
            ShowMenu(startingMenu);
        }
    }

    public Menu CreateMenu(string name, MenuButton[,] items, int? rows = null, int? columns = null) {
        Menu menu = new(name, items, rows, columns);
        RegisterMenu(menu);

        return menu;
    }

    public void RegisterMenu(Menu menu) {
        if (RegisteredMenus.Contains(menu)) return;
        RegisteredMenus.Add(menu);

        TextBlock textBlock = new TextBlock() {
            Text = menu.Name
        };

        Button button = new() {
            Content = textBlock
        };

        button.Click += (_, _) => {
            ShowMenu(menu);
            UIDispatcher.UpdateUI();
        };

        Menu.SP_MenuList.Children.Add(button);
    }

    public void UnregisterAllMenus() {
        Menu.SP_MenuList.Children.Clear();
        RegisteredMenus.Clear();
    }

    public void ShowMenu(Menu? menu)
    {
        CurrentMenu = menu;
        Rows = menu?.Rows ?? 0;
        Columns = menu?.Columns ?? 0;

        if (menu == null)
        {
            OnMenuChanged?.Invoke(null);
            return;
        }

        OnMenuChanged?.Invoke(menu);
        OrderManager.UnlockOrder();

        UIDispatcher.EnqueueOnUIThread(() => {
            Buttons.ForEach(b => MenuView.Children.Remove(b));

            for (int row = 0; row < menu.Items.GetLength(0); row++)
            {
                for (int column = 0; column < menu.Items.GetLength(1); column++)
                {
                    var item = menu.Items[row, column];
                    if (item == null) continue;

                    SetItemButton(row, column, item);
                }
            }

            MenuViewManager.ShowView(MenuView);
        });
    }

    public void ShowFirstMenu() =>
        ShowMenu(RegisteredMenus.FirstOrDefault());

    public void ShowPaymentScreen() {
        UIDispatcher.EnqueueOnUIThread(() => {
            MenuViewManager.ShowView(PaymentView);
            OnShowPaymentScreen?.Invoke();
        });
    }

    public void ShowFunctionsScreen() {
        UIDispatcher.EnqueueOnUIThread(() => {
            MenuViewManager.ShowView(FunctionsView);
        });
    }

    public void ShowKeypadScreen() {
        UIDispatcher.EnqueueOnUIThread(() => MenuViewManager.ShowView(KeypadView));
    }

    void SetItemButton(int row, int column, MenuButton item) {
        if (row >= Rows) throw new IndexOutOfRangeException();
        if (column >= Columns) throw new IndexOutOfRangeException();

        var button = item.CreateButton(Menu);
        Buttons.Add(button);

        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);

        MenuView.Children.Add(button);
    }
}
