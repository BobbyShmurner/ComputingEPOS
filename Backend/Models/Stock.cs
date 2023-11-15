using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Backend.Models;

[Table("Stock")]
public class Stock {
	[Key]
	public int StockID { get; set; }

	[Required]
	public int SupplierID { get; set; }

	[Required]
	public string? Name { get; set; }

	public float Quantity { get; set; } = 0;
}