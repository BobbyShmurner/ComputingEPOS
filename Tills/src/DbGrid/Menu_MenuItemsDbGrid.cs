using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class Menu_MenuItemsDbGrid : DbGrid<Menu_MenuItem> {
    public override string Title => "Customise Menus";

    List<Stock> Stock = new();
    List<Models.Menu> Menus = new();
    List<Models.MenuItem> MenuItems = new();

    protected override async Task<List<Menu_MenuItem>> CollectData() {
        Stock = await Api.Stock.GetStock();
        Menus = await Api.Menus.GetMenus();
        MenuItems = await Api.MenuItems.GetMenuItems();

        return await Api.Menu_MenuItems.GetMenu_MenuItems();
    }

    protected override Task<Menu_MenuItem> SaveChanges(Menu_MenuItem menu_MenuItem, bool createNew) {
        if (!createNew)
            return Api.Menu_MenuItems.PutMenu_MenuItem(menu_MenuItem);
        else
            return Api.Menu_MenuItems.Create(menu_MenuItem)!;
    }

    protected override Task Delete(Menu_MenuItem menu_MenuItem) =>
        Api.Menu_MenuItems.DeleteMenu_MenuItem(menu_MenuItem);

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

        leftFields.Add(new FkDbField<Menu_MenuItem>("Menu", nameof(Menu_MenuItem.MenuID), menuKeysAndDisplayNames));
        leftFields.Add(new FkDbField<Menu_MenuItem>("MenuItem", nameof(Menu_MenuItem.MenuItemID), menuItemsKeysAndDisplayNames));
        rightFields.Add(new IntNullDbField<Menu_MenuItem>("Row", nameof(Menu_MenuItem.Row)));
        rightFields.Add(new IntNullDbField<Menu_MenuItem>("Column", nameof(Menu_MenuItem.Column)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Menu_MenuItem.Menu_MenuItemID), width: new DataGridLength(50)),
            new DataGridColumnInfo("Menu ID", nameof(Menu_MenuItem.MenuID), width: new DataGridLength(75)),
            new DataGridColumnInfo("Menu Item ID", nameof(Menu_MenuItem.MenuItemID), width: new DataGridLength(100)),
            new DataGridColumnInfo("Row", nameof(Menu_MenuItem.Row), width: new DataGridLength(60)),
            new DataGridColumnInfo("Column", nameof(Menu_MenuItem.Column), width: new DataGridLength(60)),
            new DataGridColumnInfo("Menu", nameof(Menu_MenuItem.Column)),
            new DataGridColumnInfo("Menu Item", nameof(Menu_MenuItem.Column)),
        };
    }
}