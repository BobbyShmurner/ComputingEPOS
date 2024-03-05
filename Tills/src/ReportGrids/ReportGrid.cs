using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public enum TimeInterval {
    Hourly = 3600,
    Daily = 86400,
    Weekly = 604800,
}

public static class TimeIntervalExtensions {
    public static List<DateTime> GetIntervals(this TimeInterval interval, DateTime from, DateTime to)  {
        var intervals = new List<DateTime>();
        var current = from;

        while (current < to) {
            intervals.Add(current);
            current = current.AddSeconds((int)interval);
        }

        intervals.Add(to);
        return intervals;
    }

    public static List<DateTime> GetIntervals(this TimeInterval interval) {
        switch (interval) {
            case TimeInterval.Hourly:
                return GetIntervals(interval, DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddTicks(-1));
            case TimeInterval.Daily:
                return GetIntervals(interval, DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday).AddMonths(-1), DateTime.Today);
            case TimeInterval.Weekly:
                return GetIntervals(interval, DateTime.Now.Date.AddMonths(-6), DateTime.Now.Date);
            default:
                return new();
        };
    }
}

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

    public async Task ShowGrid(DataGrid grid, TimeInterval timeFrame) {
        Data = new ObservableCollection<T>(await CollectData(timeFrame));

        if (ColumnInfo == null)
            ColumnInfo = GetColumnInfo();

        UIDispatcher.EnqueueOnUIThread(() => {
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
        });
    }

    protected abstract Task<List<T>> CollectData(TimeInterval interval);
    protected abstract List<DataGridColumnInfo> GetColumnInfo();
}