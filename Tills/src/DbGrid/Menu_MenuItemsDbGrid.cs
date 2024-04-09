using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using Models = ComputingEPOS.Common.Models;

namespace ComputingEPOS.Tills;

public class Menu_MenuItemsDbGrid : DbGrid<Menu_MenuItemInfo> {
    public override string Title => "Customise Menus";

    List<Models.Stock> Stock = new();
    List<Models.Menu> Menus = new();
    List<Models.MenuItem> MenuItems = new();

    protected override async Task<List<Menu_MenuItemInfo>> CollectData() {
        Stock = await Api.Stock.GetStock();
        Menus = await Api.Menus.GetMenus();
        MenuItems = await Api.MenuItems.GetMenuItems();

        return (await Api.Menu_MenuItems.GetMenu_MenuItems()).Select(x => new Menu_MenuItemInfo(x, Stock, Menus, MenuItems)).ToList()!;
    }

    protected override async Task<Menu_MenuItemInfo> SaveChanges(Menu_MenuItemInfo info, bool createNew) {
        if (!createNew)
            return new Menu_MenuItemInfo(await Api.Menu_MenuItems.PutMenu_MenuItem(info), info.MenuName, info.MenuItemName);
        else
            return new Menu_MenuItemInfo((await Api.Menu_MenuItems.Create(info))!, info.MenuName, info.MenuItemName);
    }

    protected override Task Delete(Menu_MenuItemInfo info) =>
        Api.Menu_MenuItems.DeleteMenu_MenuItem(info);

    protected override void CollectFields(List<IDbField> leftFields, List<IDbField> centerFields, List<IDbField> rightFields) {
        List<(int, string)> menuKeysAndDisplayNames = Menus.Select(m => (m.MenuID, m.Name)).ToList()!;
        List<(int, string)> menuItemsKeysAndDisplayNames = MenuItems.Select(item => {
            int menuItemID = item.MenuItemID;
            Stock? stock = Stock.FirstOrDefault(s => s.StockID == item.StockID);
            string displayName;

            if (stock != null && stock.Name != null) {
                displayName = stock.Name;
            } else {
                displayName = "[UNKNOWN]";
            }

            if (item.Note != null) {
                displayName += $" ({item.Note})";
            }

            return (menuItemID, displayName);
        }).ToList()!;

        leftFields.Add(new FkDbField<Menu_MenuItemInfo>("Menu", nameof(Menu_MenuItemInfo.MenuID), menuKeysAndDisplayNames));
        leftFields.Add(new FkDbField<Menu_MenuItemInfo>("MenuItem", nameof(Menu_MenuItemInfo.MenuItemID), menuItemsKeysAndDisplayNames));
        rightFields.Add(new IntNullDbField<Menu_MenuItemInfo>("Row", nameof(Menu_MenuItemInfo.Row)));
        rightFields.Add(new IntNullDbField<Menu_MenuItemInfo>("Column", nameof(Menu_MenuItemInfo.Column)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Menu_MenuItemInfo.Menu_MenuItemID), width: new DataGridLength(50)),
            new DataGridColumnInfo("Menu Item", nameof(Menu_MenuItemInfo.MenuItemName), width: new DataGridLength(2, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Menu", nameof(Menu_MenuItemInfo.MenuName), width: new DataGridLength(1, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Row", nameof(Menu_MenuItemInfo.Row), width: new DataGridLength(60)),
            new DataGridColumnInfo("Column", nameof(Menu_MenuItemInfo.Column), width: new DataGridLength(60)),
            new DataGridColumnInfo("Menu ID", nameof(Menu_MenuItemInfo.MenuID), width: new DataGridLength(75)),
            new DataGridColumnInfo("Menu Item ID", nameof(Menu_MenuItemInfo.MenuItemID), width: new DataGridLength(100)),
        };
    }
}

public class Menu_MenuItemInfo : ICopyable<Menu_MenuItemInfo> {
    public Menu_MenuItem Item { get; set; }

    public int Menu_MenuItemID {
        get => Item.Menu_MenuItemID;
        set => Item.Menu_MenuItemID = value;
    }

    public int MenuID {
        get => Item.MenuID;
        set => Item.MenuID = value;
    }

    public int MenuItemID {
        get => Item.MenuItemID;
        set => Item.MenuItemID = value;
    }

    public int Row {
        get => Item.Row;
        set => Item.Row = value;
    }

    public int Column {
        get => Item.Column;
        set => Item.Column = value;
    }

    public string MenuName { get; set; }
    public string MenuItemName { get; set; }

    public Menu_MenuItemInfo(Menu_MenuItem item, string menuName, string menuItemName) {
        Item = item;
        MenuName = menuName;
        MenuItemName = menuItemName;
    }

    public Menu_MenuItemInfo() {
        Item = new Menu_MenuItem();
        MenuName = "";
        MenuItemName = "";
    }

    public Menu_MenuItemInfo(Menu_MenuItem item, List<Models.Stock> Stock, List<Models.Menu> Menus, List<Models.MenuItem> MenuItems) {
        Models.MenuItem? menuItem = MenuItems.FirstOrDefault(x => x.MenuItemID == item.MenuItemID);
        Models.Stock? stockItem = menuItem != null ? Stock.FirstOrDefault(x => x.StockID == menuItem.StockID) : null;
        Models.Menu? menu = Menus.FirstOrDefault(x => x.MenuID == item.MenuID);

        Item = item;
        MenuName = menu?.Name ?? "[UNKNOWN]";
        MenuItemName = stockItem?.Name ?? "[UNKNOWN]";
        if (menuItem?.Note != null) {
            MenuItemName += $" ({menuItem.Note})";
        }
    }

    public static implicit operator Menu_MenuItem(Menu_MenuItemInfo menu_MenuItemInfo) => menu_MenuItemInfo.Item;

    public Menu_MenuItemInfo Copy() => new Menu_MenuItemInfo(Item.Copy(), MenuName, MenuItemName);
}