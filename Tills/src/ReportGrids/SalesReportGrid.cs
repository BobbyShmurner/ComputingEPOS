using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class SalesReportGrid : ReportGrid<SalesReportData> {
    public override string Title => "Sales";

    protected async override Task<(List<SalesReportData>, SalesReportData)> CollectData(TimeInterval interval) {
        List<SalesReportData> data = new();

        var intervals = interval.GetIntervals();

        var grossSales = await Api.Transactions.GetGrossSalesInIntervals(intervals[0], intervals.Last(), (long)interval);

        for (int i = 0; i < intervals.Count - 1; i++)
        {
            data.Add(new SalesReportData {
                Date = intervals[i],
                Gross = grossSales[i],
                Interval = interval,
            });
        }

        var total = new SalesReportData {
            DateOverride = "Total",
            Gross = grossSales.Sum(),
            Interval = interval,
        };

        return (data, total);
    }

    protected override List<DataGridColumnInfo> GetColumnInfo(TimeInterval interval) {
        return new List<DataGridColumnInfo>
        {
            new("Date", nameof(SalesReportData.DateString)),
            new("Net", nameof(SalesReportData.Net), "£{0:n2}"),
            new("Gross", nameof(SalesReportData.Gross), "£{0:n2}"),
            new("Tax", nameof(SalesReportData.Tax), "£{0:n2}"),
        };
    }
}

public class SalesReportData {
    public DateTime Date { get; set; } = DateTime.MinValue;
    public decimal Gross { get; set; } = 0m;
    public decimal Net => Gross * .8m;
    public decimal Tax => Gross * .2m;
    public string DateString => DateOverride ?? DefaultDateString;

    public TimeInterval Interval { get; set; }

    string DefaultDateString => Date.ToString(Interval == TimeInterval.Hourly ? "HH:mm" : "dd/MM/yy");
    public string? DateOverride { get; set; } = null;
}
