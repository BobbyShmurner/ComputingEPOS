using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills; 

public class TotalGrid {
    public DataGrid Grid { get; private set; }

    public TotalGrid(DataGrid grid) {
        Grid = grid;
    }

    public void SetTotal<T>(T totalData, List<DataGridColumnInfo> info) where T : class {
        int columnCount = info.Count;

        var totalSource = new string[2][];

        totalSource[0] = new string[columnCount];
        totalSource[1] = new string[columnCount];

        Grid.Columns.Clear();

        for (int i = 0; i < columnCount; i++) {
            var columnInfo = info[i];

            var dataColumn = new DataGridTextColumn {
                Header = columnInfo.Header,
                Binding = new Binding($"[{i}]"),
                Width = columnInfo.Width,
            };

            totalSource[1][i] = string.Format(columnInfo.Format, totalData.GetType().GetProperty(columnInfo.Binding)?.GetValue(totalData) ?? "");

            Grid.Columns.Add(dataColumn);
        }

        Grid.ItemsSource = totalSource;
    }
}
