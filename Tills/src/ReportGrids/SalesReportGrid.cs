using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class SalesReportGrid : ReportGrid<SalesReportData> {
    public override string Title => "Sales";

    protected async override Task<List<SalesReportData>> CollectData(TimeInterval interval) {
        List<SalesReportData> data = new();

        var intervals = interval.GetIntervals();

        var grossSales = await Api.Transactions.GetGrossSalesInIntervals(intervals[0], intervals.Last(), (long)interval);

        for (int i = 0; i < intervals.Count - 1; i++)
        {
            data.Add(new SalesReportData {
                Date = intervals[i],
                Gross = grossSales[i],
            });
        }

        decimal grossSum = grossSales.Sum();

        return data;
    }

    protected override List<DataGridColumnInfo> GetColumnInfo(TimeInterval interval) {
        return new List<DataGridColumnInfo>
        {
            new("Date", nameof(SalesReportData.Date), interval == TimeInterval.Hourly ? "HH:mm" : "dd/MM/yy"),
            new("Net", nameof(SalesReportData.Net)),
            new("Gross", nameof(SalesReportData.Gross)),
            new("Tax", nameof(SalesReportData.Tax)),
        };
    }
}

public class SalesReportData {
    public DateTime Date { get; set; } = DateTime.MinValue;
    public decimal Gross { get; set; } = 0m;
    public decimal Net => Gross * .8m;
    public decimal Tax => Gross * .2m;
}
