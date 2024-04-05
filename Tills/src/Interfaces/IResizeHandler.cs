using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public interface IResizeHandler : IReplacedObserver {
    public Task<OrderListItemView> Resize(OrderManager manager, bool upsize);
    public ItemSize Size { get; }
}
