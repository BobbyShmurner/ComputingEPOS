using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;
using MenuItem = ComputingEPOS.Models.MenuItem;

public class MenuItemsDbGrid : DbGrid<MenuItemInfo> {
    public override string Title => "Menu Items";

    List<Stock> Stock = new();

    protected override async Task<List<MenuItemInfo>> CollectData() {
        Stock = await Api.Stock.GetStock();

        return (await Api.MenuItems.GetMenuItems())
            .Select(x => new MenuItemInfo(x, Stock))
            .ToList()!;
    }

    protected override async Task<MenuItemInfo> SaveChanges(MenuItemInfo menuItem, bool createNew) {
        if (!createNew)
            return new MenuItemInfo(await Api.MenuItems.PutMenuItem(menuItem), Stock);
        else
            return new MenuItemInfo((await Api.MenuItems.Create(menuItem))!, Stock);
    }

    protected override Task Delete(MenuItemInfo menuItem) =>
        Api.MenuItems.DeleteMenuItem(menuItem);

    protected override void CollectFields(List<IDbField> leftFields, List<IDbField> centerFields, List<IDbField> rightFields) {
        List<(int, string)> stockKeysAndDisplayNames = Stock.Select(x => (x.StockID, x.Name)).ToList()!;

        leftFields.Add(new FkDbField<MenuItemInfo>("Stock", nameof(MenuItemInfo.StockID), stockKeysAndDisplayNames));
        centerFields.Add(new StringDbField<MenuItemInfo>("Note", nameof(MenuItemInfo.Note)));
        rightFields.Add(new DecimalNullDbField<MenuItemInfo>("Price", nameof(MenuItemInfo.Price)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(MenuItemInfo.MenuItemID), width: new DataGridLength(50)),
            new DataGridColumnInfo("Stock ID", nameof(MenuItemInfo.StockID), width: new DataGridLength(75)),
            new DataGridColumnInfo("Stock Name", nameof(MenuItemInfo.StockName), width: new DataGridLength(1, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Price", nameof(MenuItemInfo.Price), format: "£{0:n2}", width: new DataGridLength(65)),
            new DataGridColumnInfo("Note", nameof(MenuItemInfo.Note), width: new DataGridLength(2, DataGridLengthUnitType.Star)),
        };
    }
}

public class MenuItemInfo : ICopyable<MenuItemInfo> {
    public int MenuItemID {
        get => Item.MenuItemID;
        set => Item.MenuItemID = value;
    }

    public int StockID {
        get => Item.StockID;
        set => Item.StockID = value;
    }

    public string Note {
        get => Item.Note ?? "";
        set => Item.Note = value == StockName || value == "" ? null : value;
    }

    public decimal? Price {
        get => Item.Price;
        set => Item.Price = value;
    }

    public string StockName { get; set; }

    public MenuItem Item { get; set; }
    public MenuItemInfo Copy() => new MenuItemInfo(Item.Copy(), StockName);

    public static implicit operator MenuItem(MenuItemInfo menuItemInfo) => menuItemInfo.Item;

    public MenuItemInfo() {
        Item = new MenuItem();
        StockName = "";
    }

    public MenuItemInfo(MenuItem menuItem, string stockName) {
        Item = menuItem;
        StockName = stockName;
    }

    public MenuItemInfo(MenuItem menuItem, List<Stock> stock) {
        Stock? stockItem = stock.FirstOrDefault(x => x.StockID == menuItem.StockID);

        Item = menuItem;
        StockName = stockItem?.Name ?? "[UNKNOWN]";
    }
}