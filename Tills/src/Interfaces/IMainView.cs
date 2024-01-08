using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills;

public interface IMainView {
    public void OnSetMainView();
    public UIElement ViewElement { get; }
}
