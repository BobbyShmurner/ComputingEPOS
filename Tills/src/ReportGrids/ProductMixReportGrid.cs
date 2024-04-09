using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Tills;

public class ProductMixReportGrid : ReportGrid<ProductMixReportData> {
    public override string Title => "Product Mix";

    protected async override Task<(List<ProductMixReportData>, ProductMixReportData)> CollectData(TimeInterval interval) {
        List<ProductMixReportData> data = new();

        List<PmixReport> reports = await Api.Stock.GetAllStockPmix(interval.GetFromDate(), DateTime.Now);

        foreach (PmixReport report in reports) {
            data.Add(new ProductMixReportData {
                Stock = report.Stock.Name ?? "UNKNOWN",
                QuantitySold = report.QuantitySold,
                Gross = report.Gross,
            });
        }

        ProductMixReportData total = new() {
            Stock = "Total",
            QuantitySold = data.Sum(x => x.QuantitySold),
            Gross = data.Sum(x => x.Gross),
        };

        return (data, total);
    }

    protected override List<DataGridColumnInfo> GetColumnInfo(TimeInterval interval) {
        return new List<DataGridColumnInfo>
        {
            new("Stock", nameof(ProductMixReportData.Stock)),
            new("QuantitySold", nameof(ProductMixReportData.QuantitySold)),
            new("Net", nameof(ProductMixReportData.Net), "£{0:n2}"),
            new("Gross", nameof(ProductMixReportData.Gross), "£{0:n2}"),
            new("Tax", nameof(ProductMixReportData.Tax), "£{0:n2}"),
        };
    }
}

public class ProductMixReportData {
    public string Stock { get; set; } = string.Empty;
    public float QuantitySold { get; set; } = 0;
    public decimal Gross { get; set; } = 0m;
    public decimal Net => Gross * .8m;
    public decimal Tax => Gross * .2m;
}
