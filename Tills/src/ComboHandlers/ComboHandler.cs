using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class ComboHandler : IComboHandler {
    public bool IsCombo { get; private set; }

    public ComboHandler(OrderListItemView itemView) {
        RootItemView = itemView;
    }

    public OrderListItemView RootItemView { get; private set; }
    public OrderListItemView MainItemView => IsCombo ? RootItemView.Children[0] : RootItemView;
    public OrderListItemView? DrinkItemView => IsCombo ? RootItemView.Children[1] : null;
    public OrderListItemView? SideItemView => IsCombo ? RootItemView.Children[2] : null;

    public void OnReplaced(OrderListItemView oldView, OrderListItemView newView) {
        if (oldView == RootItemView)
            RootItemView = newView;
            
        RepairReferences();
    }

    public Task<OrderListItemView> Combo(OrderManager manager) =>
        IsCombo ? DeCombo(manager) : MakeCombo(manager);

    async Task<ItemSize?> GetOgSizeAndRestSize(OrderManager manager) {
        ItemSize? ogSize = null;
        if (RootItemView.ResizeHandler != null) {
            ogSize = RootItemView.ResizeHandler.Size;

            switch (ogSize) {
                case ItemSize.Small:
                    await RootItemView.ResizeHandler.Resize(manager, true);
                    break;
                case ItemSize.Large:
                    await RootItemView.ResizeHandler.Resize(manager, false);
                    break;
                default:
                    break;
            }
        }

        return ogSize;
    }

    async Task ResetSizeToOg(ItemSize ogSize, OrderManager manager) {
        switch (ogSize) {
            case ItemSize.Small:
                RootItemView = await RootItemView.ResizeHandler!.Resize(manager, false);
                break;
            case ItemSize.Large:
                RootItemView = await RootItemView.ResizeHandler!.Resize(manager, true);
                break;
            default:
                break;
        }
    }

    async Task<OrderListItemView> MakeCombo(OrderManager manager) {
        ItemSize? ogSize = await GetOgSizeAndRestSize(manager);

        var itemClone = MainItemView.Item.Clone();
        var drinkItem = new OrderListItem("Coke", 17, 0.50M);
        var sideItem = new OrderListItem("Fries", 23, 0.50M);
        var mealItem = new OrderListItem(itemClone.Text + " Meal", itemClone, drinkItem, sideItem);

        IsCombo = true;
        await manager.ReplaceOrderItem(RootItemView, mealItem);

        if (ogSize != null) await ResetSizeToOg(ogSize.Value, manager);
        return RootItemView;
    }

    public void RepairReferences() {
        RootItemView.AllChildren.ForEach(c => c.ComboHandler = this);
        RootItemView.ComboHandler = this;

        MainItemView.DeletionTarget = RootItemView;
        if (SideItemView != null) SideItemView.DeletionTarget = RootItemView;
        if (DrinkItemView != null) DrinkItemView.DeletionTarget = RootItemView;
    }

    async Task<OrderListItemView> DeCombo(OrderManager manager) {
        ItemSize? ogSize = await GetOgSizeAndRestSize(manager);

        var itemClone = MainItemView.Item.Clone();
        IsCombo = false;
        
        await manager.ReplaceOrderItem(RootItemView, itemClone);

        if (ogSize != null) await ResetSizeToOg(ogSize.Value, manager);
        return RootItemView;
    }
}
