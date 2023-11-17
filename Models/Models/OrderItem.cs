using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("OrderItems")]
public class OrderItem {
	[Key]
	public int OrderItemID { get; set; }

	[Required]
	public int OrderID { get; set; }

	[Required]
	public int StockID { get; set; }

	[Required]
	public float Quantity { get; set; }

	[Required]
	public decimal Subtotal { get; set; }
}