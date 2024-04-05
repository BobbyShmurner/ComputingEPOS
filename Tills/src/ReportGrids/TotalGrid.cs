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

    public static string[][] GenerateRowData<T>(T totalData, List<DataGridColumnInfo> info) where T : class {
        int columnCount = info.Count;

        string[][] totalRows = [new string[columnCount], new string[columnCount]];

        for (int i = 0; i < columnCount; i++) {
            var columnInfo = info[i];
            totalRows[1][i] = string.Format(columnInfo.Format, totalData.GetType().GetProperty(columnInfo.Binding)?.GetValue(totalData) ?? "");
        }

        return totalRows;
    }

    public void SetTotal<T>(T totalData, List<DataGridColumnInfo> info) where T : class {
        int columnCount = info.Count;
        string[][] totalRows = GenerateRowData(totalData, info);

        Grid.Columns.Clear();

        for (int i = 0; i < columnCount; i++) {
            var columnInfo = info[i];

            var dataColumn = new DataGridTextColumn {
                Header = columnInfo.Header,
                Binding = new Binding($"[{i}]"),
                Width = columnInfo.Width,
            };

            Grid.Columns.Add(dataColumn);
        }

        Grid.ItemsSource = totalRows;
    }
}
