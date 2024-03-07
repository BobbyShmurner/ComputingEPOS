using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static ComputingEPOS.Models.Transaction;

namespace ComputingEPOS.Tills;

public enum CheckoutType
{
    SitIn,
    TakeAway
}

public static class CheckoutTypeExtensions
{
    public static string ToPrettyString(this CheckoutType checkoutType)
    {
        switch (checkoutType)
        {
            case CheckoutType.SitIn: return "Sit In";
            case CheckoutType.TakeAway: return "Take Away";
            default: return checkoutType.ToString();
        }
    }
}

public class OrderManager : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<OrderListItemView?>? OnSelectionChanged;

    public MenuView Menu { get; private set; }
    public OrderMenuManager OrderMenuManager => Menu.OrderMenuManager;

    public List<OrderListItemView> RootItems { get; private set; } = [];
    public Dictionary<OrderListItem, OrderListItemView> Views = [];

    public bool IsItemSelected => Selected != null;

    public bool CanDeleteSelected => Selected?.DeletionTarget != null;
    public bool CanComboSelected => Selected?.ComboHandler != null;
    public bool CanDownsizeSelected => IsItemSelected;
    public bool CanModifySelected => IsItemSelected;
    public bool CanUpsizeSelected => IsItemSelected;

    OrderListItemView? m_Selected = null;
    public OrderListItemView? Selected {
        get => m_Selected;
        private set {
            m_Selected = value;
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected))));
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsItemSelected))));

            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDownsizeSelected))));
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDeleteSelected))));
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUpsizeSelected))));
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanModifySelected))));
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanComboSelected))));
        }
    }

    bool m_IsOrderLocked = false;
    public bool IsOrderLocked {
        get => m_IsOrderLocked;
        private set {
            m_IsOrderLocked = value;
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOrderLocked))));
        }
    }

    decimal m_Total = 0;
    public decimal Total {
        get => m_Total;
        set {
            m_Total = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubTotal)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tax)));
            });
        }
    }

    decimal m_AmountPaid = 0;
    public decimal AmountPaid {
        get => m_AmountPaid;
        set {
            m_AmountPaid = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmountPaid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Outstanding)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutstandingStr)));
            });
        }
    }

    public string OpenCheckCountStr => OpenChecks == null
        ? "Fetching..."
        : $"{OpenChecks.Value} Open {(OpenChecks.Value != 1 ? "Checks" : "Check")}";

    int? m_OpenChecks = null;
    public int? OpenChecks {
        get => m_OpenChecks;
        set {
            m_OpenChecks = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenChecks)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenCheckCountStr)));
            });
        }
    }

    public int OrderNumber => CurrentOrder?.OrderNum ?? 0;

    CheckoutType? m_CheckoutType;
    public CheckoutType? CheckoutType {
        get => m_CheckoutType;
        set {
            m_CheckoutType = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckoutType)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckoutTypePretty)));
            });
        }
    }

    PaymentMethods? m_PaymentMethod;
    public PaymentMethods? PaymentMethod {
        get => m_PaymentMethod;
        set {
            m_PaymentMethod = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentMethod)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentMethodPretty)));
            });
        }
    }

    public string CheckoutTypePretty => CheckoutType?.ToPrettyString() ?? "None";
    public string PaymentMethodPretty => PaymentMethod?.ToString() ?? "None";

    public decimal SubTotal => Total * 0.8M;
    public decimal Tax => Total * 0.2M;
    public decimal Outstanding => Total - AmountPaid;

    public string OutstandingStr => FetchingAmountPaid
        ? "Outstanding: ..."
        : $"Outstanding: £{Outstanding:n2}";

    bool m_FetchingAmountPaid = false;
    public bool FetchingAmountPaid {
        get => m_FetchingAmountPaid;
        set {
            m_FetchingAmountPaid = value;

            UIDispatcher.EnqueueOnUIThread(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FetchingAmountPaid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutstandingStr)));
            });
        }
    }

    Order? m_CurrentOrder;
    public Order? CurrentOrder {
        get => m_CurrentOrder;
        set {
            m_CurrentOrder = value;
            UIDispatcher.EnqueueOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderNumber))));
        }
    }

    public OrderManager(MenuView menu) {
        Menu = menu;
        OrderMenuManager.OnMenuChanged += menu => IsOrderLocked = menu != null;

        Task.Run(async () => {
            while (true) {
                Thread.Sleep(3000);
                await FetchOpenChecks();

                // Only update UI if this is the only queued Ui action
                if (UIDispatcher.Instance.queuedUiActions == 0 && UIDispatcher.Instance.queuedUiThreadActions == 1)
                    UIDispatcher.UpdateUI();
                else
                    await UIDispatcher.WaitForUIUpdate();
            }
        });
    }

    public async Task NextOrder() {
        await DeleteAllItems(false);

        while (true) {
            try {
                CurrentOrder = await Api.Orders.Create();
                break;
            } catch {
                continue;
            }
        }

        OrderMenuManager.ShowFirstMenu();
        UnlockOrder();

        await FetchOpenChecks();
    }

    public async Task DeleteCurrentOrder() {
        if (CurrentOrder == null) throw new ArgumentNullException(nameof(CurrentOrder), "No order to delete!");

        try {
            await Api.Orders.DeleteOrder(CurrentOrder);
        } catch (Exception ex) {
            Trace.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task CloseAllPaidChecks(bool closeEmpty = false) {
        try {
            await Api.Orders.CloseAllPaidChecks(closeEmpty);
        } catch (Exception ex) {
            Trace.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<OrderListItemView> AddOrderItem(OrderListItem item, OrderListItemView? parent = null, int? index = null) {
        if (CurrentOrder == null) throw new ArgumentNullException(nameof(CurrentOrder), "No order to add item to!");
        var itemClone = item.Clone();

        if (item.StockID != null) {
            try {
                var returnedOrderItem = await Api.OrderItems.Create(CurrentOrder.OrderID, item.StockID.Value, 1, item.Price);
                itemClone.OrderItem = returnedOrderItem;
            } catch (HttpRequestException ex) {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        item = itemClone;

        OrderListItemView view = new OrderListItemView(this, Menu, item, parent, index);
        Views[item] = view;

        if (item.Price.HasValue) Total += item.Price.Value;
        if (parent == null) RootItems.Insert(index ?? RootItems.Count, view);

        try {
            foreach (var child in view.Item.Children)
                await AddOrderItem(child, view);
        } catch {
            // Keep trying to remove this item. If it fails more than 10 times, just exit
            for (int i = 0; i < 10; i++) {
                try { await RemoveOrderItem(view, true); } catch { Thread.Sleep(100); }
            }

            throw;
        }

        SelectItem(view);
        return view;
    }

    public async Task<OrderListItemView?> MakeSelectedCombo() =>
        Selected != null && Selected.ComboHandler != null ? await Selected.ComboHandler.Combo(this) : null;

    public async Task<OrderListItemView> ReplaceOrderItem(OrderListItemView toReplace, OrderListItem replaceWith) {
        int index = toReplace.Index;
        var parent = toReplace.Parent;

        await RemoveOrderItem(toReplace, true);

        try {
            return await AddOrderItem(replaceWith, parent, index);
        } catch {
            await AddOrderItem(toReplace.Item, parent, index);
            throw;
        }
    }

    public OrderListItemView? GetNextOrderItemForSelection(OrderListItemView view) {
        if (view.Parent != null) return view.Parent;
        int index = RootItems.IndexOf(view);

        if (index < RootItems.Count - 1) return RootItems[index + 1];
        if (index > 0) return RootItems[index - 1];

        return null;
    }

    public async Task RemoveSelectedOrderItem(bool removeFromDB) {
        if (Selected == null) return;
        var target = Selected.DeletionTarget;
        if (target == null) return;

        OrderListItemView? nextSelection = GetNextOrderItemForSelection(target);

        await RemoveOrderItem(target, removeFromDB);
        SelectItem(nextSelection);
    }

    public async Task RemoveOrderItem(OrderListItemView view, bool removeFromDB) {
        if (removeFromDB && view.Item.OrderItem != null) {
            try {
                await Api.OrderItems.Delete(view.Item.OrderItem);
            } catch (Exception ex) {
                Trace.WriteLine(ex);
                throw;
            }
        }

        if (view.Parent == null) RootItems.Remove(view);
        if (view.Price.HasValue) Total -= view.Price.Value;

        while (view.Children.Count > 0)
            await RemoveOrderItem(view.Children[0], removeFromDB);

        if (Selected == view) DeselectItem();
        view.Remove();
    }

    public async Task DeleteAllItems(bool removeFromDB) {
        if (CurrentOrder == null) return;
        while (RootItems.Count > 0) await RemoveOrderItem(RootItems[0], removeFromDB);
    }

    public void DeselectItem(bool fireEvent = true) {
        RootItems.ForEach(item => item.RecursivlyHideBorder());
        Selected = null;

        if (fireEvent) OnSelectionChanged?.Invoke(null);
    }

    public async Task ForceCloseAllChecks() {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            Modal.Instance.Show("Closing Checks...", false);
        });

        await Api.Orders.ForceCloseAllChecks();
        await NextOrder();

        UIDispatcher.EnqueueOnUIThread(() => Modal.Instance.Show("Successfuly Closed All Checks!"));
    }

    public async Task FetchOpenChecks() {
        OpenChecks = (await Api.Orders.GetOpenChecks()).Count;
    }

    public async Task CheckoutOrder(CheckoutType checkoutType) {
        if (CurrentOrder == null) throw new ArgumentNullException(nameof(CurrentOrder), "No order to checkout!");
        LockOrder();
        CheckoutType = checkoutType;

        CurrentOrder = await Api.Orders.FinaliseOrder(CurrentOrder);
        await FetchAmountPaid();
        OrderMenuManager.ShowPaymentScreen();
    }

    public async Task FetchAmountPaid() =>
        AmountPaid = CurrentOrder != null ? (await Api.Orders.GetAmountPaid(CurrentOrder)).Value : 0;

    public async Task PayForOrder(decimal? amount, PaymentMethods paymentMethod, TransactionButton.SpecialFunctions special) {
        PaymentMethod = paymentMethod;

        switch (special)
        {
            case TransactionButton.SpecialFunctions.Remaning:
                amount = Outstanding;
                break;
            case TransactionButton.SpecialFunctions.RoundUp:
                amount = Math.Ceiling(Outstanding);
                break;
            case TransactionButton.SpecialFunctions.Specific:
                amount = await GetAmountKeypad();
                Trace.WriteLine($"Got amount: £{amount}");
                break;
            default:
                if (amount == null) throw new ArgumentNullException("amount");
                break;
        }

        if (paymentMethod == PaymentMethods.Card && amount.Value > Outstanding) {
            UIDispatcher.EnqueueOnUIThread(() =>
                Modal.Instance.Show($"Cannot overcharge card!\n\nInputed Amount: £{amount.Value:n2}\nOutstanding Balance: £{Outstanding:n2}")
            );

            return;
        }

        FetchingAmountPaid = true;
        UIDispatcher.UpdateUI();
        
        await PayForOrder_Internal(amount.Value, paymentMethod);

        if (Outstanding <= 0)
        {
            if (Outstanding < 0) await HandleChange();
            await CloseCheck();
            await NextOrder();
        }

        FetchingAmountPaid = false;
    }

    async Task<decimal> GetAmountKeypad() {
        bool waitingForConfirmation = true;

        EventHandler action = (_, _) => waitingForConfirmation = false;

        UIDispatcher.EnqueueOnUIThread(() => {
            Menu.PaymentKeypad.Confirm += action;
            Menu.PaymentKeypad.ClearVaule();

            OrderMenuManager.ShowKeypadScreen();
        });

        UIDispatcher.UpdateUI();

        while (waitingForConfirmation) {
            await Task.Delay(100);
        }

        int value = 0;

        UIDispatcher.EnqueueOnUIThread(() => {
            value = Menu.PaymentKeypad.Value;
            Menu.PaymentKeypad.Confirm -= action;

            OrderMenuManager.ShowPaymentScreen();
        });

        UIDispatcher.UpdateUI();
        return value / 100M;
    }

    async Task HandleChange() {
        decimal change = -Outstanding;

        await PayForOrder_Internal(Outstanding, PaymentMethods.Cash);
        UIDispatcher.EnqueueOnUIThread(() => Modal.Instance.Show($"Change: £{change}"));
    }

    async Task CloseCheck() {
        if (CurrentOrder == null) throw new ArgumentNullException(nameof(CurrentOrder), "No order to close!");
        CurrentOrder = await Api.Orders.CloseCheck(CurrentOrder, false);
    }

    async Task PayForOrder_Internal(decimal amount, PaymentMethods paymentMethod) {
        if (CurrentOrder == null) throw new ArgumentNullException(nameof(CurrentOrder), "No order to pay for!");

        await Api.Transactions.Create(CurrentOrder, amount, paymentMethod);
        await FetchAmountPaid();
    }

    public void LockOrder()
    {
        IsOrderLocked = true;
        DeselectItem();
    }

    public void UnlockOrder()
    {
        IsOrderLocked = false;
        SelectLastItem();
    }

    public void SelectItem(OrderListItemView? view)
    {
        DeselectItem(false);
        if (view == null) return;
        if (IsOrderLocked) return;

        view.Selected = true;
        Selected = view;

        OnSelectionChanged?.Invoke(Selected);
    }

    public void SelectLastItem() => SelectItem(RootItems.LastOrDefault());
}
