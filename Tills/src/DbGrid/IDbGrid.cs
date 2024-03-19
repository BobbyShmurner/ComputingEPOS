using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public interface IDbGrid {
    public Type Type { get; }
    public string Title { get; }

    public void HideGrid();
    public Task ShowGrid(DataGrid grid, DataGrid addGrid, StackPanel leftPanel, StackPanel centerPanel, StackPanel rightPanel, bool resetSelection);
    public Task SaveChanges();
    public Task DeleteCurrent();
}