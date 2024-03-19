using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Stock")]
public class Stock : ICopyable<Stock> {
	[Key]
	public int StockID { get; set; }

	[Required]
	public int SupplierID { get; set; }

	[Required]
	public string? Name { get; set; }

	public float Quantity { get; set; } = 0;

    public Stock Copy() => new Stock {
        StockID = StockID,
		SupplierID = SupplierID,
		Name = Name,
		Quantity = Quantity,
    };
}