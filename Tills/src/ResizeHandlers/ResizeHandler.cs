using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class ResizeHandler(OrderListItemView itemView) : IResizeHandler {
    public void OnReplaced(OrderListItemView oldView, OrderListItemView newView) {
        if (itemView != oldView) return;
        itemView = newView;
    }

    public async Task<OrderListItemView> Resize(OrderManager manager, bool upsize) {
        // if (itemView.Parent != null && (itemView.Parent.ComboHandler?.IsCombo ?? false)) {
        //     itemView = itemView.Parent;
        //     return await Resize(manager, upsize);
        // }


        if (!(itemView.ComboHandler?.IsCombo ?? false)) {
            return await ResizeItem(manager, itemView, upsize);
        }

        await ResizeItem(manager, itemView.ComboHandler.SideItemView!, upsize);
        await ResizeItem(manager, itemView.ComboHandler.DrinkItemView!, upsize);
        RenameMealItem(itemView.ComboHandler.RootItemView!, upsize);

        return itemView.ComboHandler.RootItemView!;
    }

    public ItemSize Size => GetCurrentSize(itemView.Item.Text);

    ItemSize GetCurrentSize(string name) =>
        name.StartsWith("Large") ? ItemSize.Large : name.StartsWith("Small") ? ItemSize.Small : ItemSize.Regular;

    ItemSize GetNewSize(ItemSize size, bool upsize) =>
        size switch {
            ItemSize.Small => upsize ? ItemSize.Regular : ItemSize.Small,
            ItemSize.Regular => upsize ? ItemSize.Large : ItemSize.Small,
            ItemSize.Large => upsize ? ItemSize.Large : ItemSize.Regular,
            _ => throw new InvalidOperationException("Invalid item size!")
        };

    string GetNewName(string name, ItemSize newSize) {
        string trimmedName = name.TrimStart("Small ").TrimStart("Large ");
        switch (newSize) {
            case ItemSize.Small:
                trimmedName = $"Small {trimmedName}";
                break;
            case ItemSize.Large:
                trimmedName = $"Large {trimmedName}";
                break;
            default:
                break;
        }

        return trimmedName;
    }

    decimal GetPriceDelta(ItemSize size, bool upsize) =>
        size switch {
            ItemSize.Small => upsize ? 0.25M : 0,
            ItemSize.Regular => upsize ? 0.5M : -0.25M,
            ItemSize.Large => upsize ? 0 : -0.5M,
            _ => throw new InvalidOperationException("Invalid item size!")
        };

    void RenameMealItem(OrderListItemView itemView, bool upsize) {
        ItemSize size  = GetCurrentSize(itemView.Item.Text);
        ItemSize newSize = GetNewSize(size, upsize);

        itemView.Text = GetNewName(itemView.Item.Text, newSize);
        itemView.Item.RenameText(itemView.Text);
    }

    async Task<OrderListItemView> ResizeItem(OrderManager manager, OrderListItemView itemView, bool upsize) {
        if (upsize && !manager.CanUpsizeItem(itemView)) return itemView;
        if (!upsize && !manager.CanDownsizeItem(itemView)) return itemView;

        OrderListItem item = itemView.Item;
        ItemSize size  = GetCurrentSize(itemView.Item.Text);
        ItemSize newSize = GetNewSize(size, upsize);

        string newName = GetNewName(itemView.Item.Text, newSize);

        OrderListItem newItem = item.NewFrom(newName, item.StockID ?? -1, GetPriceDelta(size, upsize));
        return await manager.ReplaceOrderItem(itemView, newItem);
    }
}
