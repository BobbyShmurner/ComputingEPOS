using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class StockDbGrid : DbGrid<Stock> {
    public override string Title => "Stock";

    protected override Task<List<Stock>> CollectData() =>
        Api.Stock.GetStock();

    protected override Task<Stock> SaveChanges(Stock stock, bool createNew) {
        if (!createNew)
            return Api.Stock.PutStock(stock);
        else
            return Api.Stock.Create(stock)!;
    }

    protected override Task Delete(Stock stock) =>
        Api.Stock.DeleteStock(stock);

    protected override void CollectFields(List<IDbField> leftFields, List<IDbField> centerFields, List<IDbField> rightFields) {
        leftFields.Add(new StringDbField<Stock>("Name", nameof(Stock.Name)));
        rightFields.Add(new FloatDbField<Stock>("Quantity", nameof(Stock.Quantity)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Stock.StockID), width: new DataGridLength(50)),
            new DataGridColumnInfo("Supplier ID", nameof(Stock.SupplierID), width: new DataGridLength(75)),
            new DataGridColumnInfo("Name", nameof(Stock.Name)),
            new DataGridColumnInfo("Quantity", nameof(Stock.Quantity)),
        };
    }
}