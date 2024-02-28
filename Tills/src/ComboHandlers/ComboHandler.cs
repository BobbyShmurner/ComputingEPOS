using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class ComboHandler(OrderListItemView itemView) : IComboHandler {
    public bool IsCombo { get; private set; }

    public OrderListItemView RootItemView { get; private set; } = itemView;
    public OrderListItemView MainItemView => IsCombo ? RootItemView.Children[0] : RootItemView;
    public OrderListItemView? DrinkItemView => IsCombo ? RootItemView.Children[1] : null;
    public OrderListItemView? SideItemView => IsCombo ? RootItemView.Children[2] : null;

    public Task<OrderListItemView> Combo(OrderManager manager) =>
        IsCombo ? DeCombo(manager) : MakeCombo(manager);

    async Task<OrderListItemView> MakeCombo(OrderManager manager) {
        var itemClone = MainItemView.Item.Clone();
        var drinkItem = new OrderListItem("Coke", 17, 0.50M);
        var sideItem = new OrderListItem("Fries", 23, 0.50M);
        var mealItem = new OrderListItem(itemClone.Text + " Meal", itemClone, drinkItem, sideItem);

        RootItemView = await manager.ReplaceOrderItem(MainItemView, mealItem);
        RootItemView.AllChildren.ForEach(c => c.ComboHandler = this);

        RootItemView.ComboHandler = this;
        IsCombo = true;

        MainItemView.DeletionTarget = RootItemView;
        SideItemView!.DeletionTarget = RootItemView;
        DrinkItemView!.DeletionTarget = RootItemView;

        return RootItemView;
    }

    async Task<OrderListItemView> DeCombo(OrderManager manager) {
        var itemClone = MainItemView.Item.Clone();
        RootItemView = await manager.ReplaceOrderItem(RootItemView, itemClone);
        RootItemView.AllChildren.ForEach(c => c.ComboHandler = this);
        RootItemView.ComboHandler = this;

        IsCombo = false;
        return RootItemView;
    }
}
