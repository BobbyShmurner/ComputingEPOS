using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public struct DataGridColumnInfo
{
    public string Header { get; }
    public string Binding { get; }
    public DataGridLength Width { get; }

    public DataGridColumnInfo(string header, string binding)
    {
        Header = header;
        Binding = binding;
        Width = new(1.0, System.Windows.Controls.DataGridLengthUnitType.Star);
    }

    public DataGridColumnInfo(string header, string binding, DataGridLength width) {
        Header = header;
        Binding = binding;
        Width = width;
    }
}

public abstract class ReportGrid<T> : IReportGrid where T : class {
    public ObservableCollection<T>? Data { get; private set; }
    public List<DataGridColumnInfo>? ColumnInfo { get; private set; }
    public abstract string Title { get; }

    public Type Type => typeof(T);

    public void ShowGrid(DataGrid grid) {
        if (Data == null)
            Data = new ObservableCollection<T>(CollectData());

        if (ColumnInfo == null)
            ColumnInfo = GetColumnInfo();

        grid.ItemsSource = Data;
        grid.Columns.Clear();

        foreach (var columnInfo in ColumnInfo)
        {
            var dataColumn = new DataGridTextColumn
            {
                Header = columnInfo.Header,
                Binding = new Binding(columnInfo.Binding),
                Width = columnInfo.Width,
            };
            grid.Columns.Add(dataColumn);
        }
    }

    protected abstract List<T> CollectData();
    protected abstract List<DataGridColumnInfo> GetColumnInfo();
}