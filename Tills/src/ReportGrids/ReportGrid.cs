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
    Monthly = 2592000,
    Annually = 31536000,
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
                return GetIntervals(interval, DateTime.Now.Date.AddMonths(-4), DateTime.Now.Date);
            case TimeInterval.Monthly:
                return GetIntervals(interval, DateTime.Now.Date.AddMonths(-12), DateTime.Now.Date);
            case TimeInterval.Annually:
                return GetIntervals(interval, DateTime.Now.Date.AddYears(-6), DateTime.Now.Date);
            default:
                return new();
        };
    }

    public static DateTime GetFromDate(this TimeInterval interval, DateTime? to = null) {
        to ??= DateTime.Now;

        return interval switch {
            TimeInterval.Hourly => to.Value.AddHours(-1),
            TimeInterval.Daily => to.Value.AddDays(-1),
            TimeInterval.Weekly => to.Value.AddDays(-7),
            TimeInterval.Monthly => to.Value.AddMonths(-1),
            TimeInterval.Annually => to.Value.AddYears(-1),
            _ => to.Value,
        };
    }
}

public struct DataGridColumnInfo {
    public string Header { get; }
    public string Binding { get; }
    public string Format { get; }
    public DataGridLength Width { get; }

    public DataGridColumnInfo(string header, string binding, string format = "{0}", DataGridLength? width = null) {
        Header = header;
        Binding = binding;
        Format = format;
        Width = width ?? new(1.0, DataGridLengthUnitType.Star);
    }
}

public abstract class ReportGrid<T> : IReportGrid where T : class {
    public ObservableCollection<T>? Data { get; private set; }
    public List<DataGridColumnInfo>? ColumnInfo { get; private set; }
    public abstract string Title { get; }

    public Type Type => typeof(T);

    public async Task ShowGrid(DataGrid grid, TotalGrid totalGrid, TimeInterval timeFrame) {
        (var data, var totalData) = await CollectData(timeFrame);
        Data = new ObservableCollection<T>(data);

        ColumnInfo = GetColumnInfo(timeFrame);

        UIDispatcher.EnqueueOnUIThread(() => {
            grid.ItemsSource = Data;
            grid.Columns.Clear();

            totalGrid.SetTotal<T>(totalData, ColumnInfo);

            foreach (var columnInfo in ColumnInfo) {
                var dataColumn = new DataGridTextColumn {
                    Header = columnInfo.Header,
                    Binding = new Binding(columnInfo.Binding) { StringFormat = columnInfo.Format},
                    Width = columnInfo.Width,
                };

                grid.Columns.Add(dataColumn);
            }
        });
    }

    protected abstract Task<(List<T>, T)> CollectData(TimeInterval interval);
    protected abstract List<DataGridColumnInfo> GetColumnInfo(TimeInterval interval);
}