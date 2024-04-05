using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public interface IComboHandler : IReplacedObserver {
    public Task<OrderListItemView> Combo(OrderManager manager);

    public bool IsCombo { get; }
    public OrderListItemView RootItemView { get; }
    public OrderListItemView MainItemView { get; }
    public OrderListItemView? DrinkItemView { get; }
    public OrderListItemView? SideItemView { get; }
}
