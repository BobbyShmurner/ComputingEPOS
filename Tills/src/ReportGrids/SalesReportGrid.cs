using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Tills;

public class SalesReportGrid : ReportGrid<SalesReportData> {
    public override string Title => "Sales";

    protected override List<SalesReportData> CollectData() {
        return new List<SalesReportData> {
            new SalesReportData
            {
                Date = "01/01/2021",
                Net = "£100.00",
                Gross = "£120.00",
                Tax = "£20.00",
                LabourCost = "£10.00"
            },
            new SalesReportData
            {
                Date = "02/01/2021",
                Net = "£200.00",
                Gross = "£240.00",
                Tax = "£40.00",
                LabourCost = "£20.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.00"
            },
            new SalesReportData
            {
                Date = "03/01/2021",
                Net = "£300.00",
                Gross = "£360.00",
                Tax = "£60.00",
                LabourCost = "£30.0054504395034950349030930090593405903495034905"
            }
        };
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo>
        {
            new("Data", nameof(SalesReportData.Date)),
            new("Net", nameof(SalesReportData.Net)),
            new("Gross", nameof(SalesReportData.Gross)),
            new("Tax", nameof(SalesReportData.Tax)),
            new("Labour Cost", nameof(SalesReportData.LabourCost))
        };
    }
}

public class SalesReportData {
    public string Date { get; set; } = string.Empty;
    public string Net { get; set; } = string.Empty;
    public string Gross { get; set; } = string.Empty;
    public string Tax { get; set; } = string.Empty;
    public string LabourCost { get; set; } = string.Empty;
}
