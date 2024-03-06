using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class ProductMixReportGrid : ReportGrid<ProductMixReportData> {
    public override string Title => "Product Mix";

    protected async override Task<List<ProductMixReportData>> CollectData(TimeInterval interval) {
        List<ProductMixReportData> data = new();

        var intervals = interval.GetIntervals();

        var grossSales = await Api.Transactions.GetGrossSalesInIntervals(intervals[0], intervals.Last(), (long)interval);

        for (int i = 0; i < intervals.Count - 1; i++)
        {
            data.Add(new ProductMixReportData {
                Stock = intervals[i].ToString(interval == TimeInterval.Hourly ? "HH:mm" : "dd/MM/yy"),
                Net = $"£{grossSales[i] * .8m:n2}",
                Gross = $"£{grossSales[i]:n2}",
                Tax = $"£{grossSales[i] * .2m:n2}",
            });
        }

        decimal grossSum = grossSales.Sum();

        data.Add(new ProductMixReportData());
        data.Add(new ProductMixReportData {
            Stock = "Total",
            Net = $"£{grossSum * .8m:n2}",
            Gross = $"£{grossSum:n2}",
            Tax = $"£{grossSum * .2m:n2}",
        });

        return data;
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo>
        {
            new("Stock", nameof(ProductMixReportData.Stock)),
            new("QuantitySold", nameof(ProductMixReportData.QuantitySold)),
            new("Net", nameof(ProductMixReportData.Net)),
            new("Gross", nameof(ProductMixReportData.Gross)),
            new("Tax", nameof(ProductMixReportData.Tax)),
        };
    }
}

public class ProductMixReportData {
    public string Stock { get; set; } = string.Empty;
    public string QuantitySold { get; set; } = string.Empty;
    public string Net { get; set; } = string.Empty;
    public string Gross { get; set; } = string.Empty;
    public string Tax { get; set; } = string.Empty;
}
