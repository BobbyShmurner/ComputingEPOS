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
                Date = intervals[i].ToString(interval == TimeInterval.Hourly ? "HH:mm" : "dd/MM/yy"),
                Net = $"£{grossSales[i] * .8m:n2}",
                Gross = $"£{grossSales[i]:n2}",
                Tax = $"£{grossSales[i] * .2m:n2}",
            });
        }

        decimal grossSum = grossSales.Sum();

        data.Add(new SalesReportData());
        data.Add(new SalesReportData {
            Date = "Total",
            Net = $"£{grossSum * .8m:n2}",
            Gross = $"£{grossSum:n2}",
            Tax = $"£{grossSum * .2m:n2}",
        });

        return data;
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo>
        {
            new("Data", nameof(SalesReportData.Date)),
            new("Net", nameof(SalesReportData.Net)),
            new("Gross", nameof(SalesReportData.Gross)),
            new("Tax", nameof(SalesReportData.Tax)),
        };
    }
}

public class SalesReportData {
    public string Date { get; set; } = string.Empty;
    public string Net { get; set; } = string.Empty;
    public string Gross { get; set; } = string.Empty;
    public string Tax { get; set; } = string.Empty;
}
