using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public interface IReportGrid {
    public Type Type { get; }
    public string Title { get; }

    public void ShowGrid(DataGrid grid);
}