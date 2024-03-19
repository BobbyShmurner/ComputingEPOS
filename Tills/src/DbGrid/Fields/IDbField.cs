using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills; 

public interface IDbField {
    public Type Type { get; }
    public void SetData(object data);
    public void UpdateData(ref object data);

    public FrameworkElement Display();
}
