using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public interface IReplacedObserver {
    public void OnReplaced(OrderListItemView oldView, OrderListItemView newView);
}
