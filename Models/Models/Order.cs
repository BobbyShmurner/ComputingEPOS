using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComputingEPOS.Attributes;

namespace ComputingEPOS.Models;


[Table("Orders")]
public class Order {
	const int DAY_IN_SECONDS = 24 * 60 * 60;

	[Key]
	public int OrderID { get; set; }

	[Required]
	public int EmployeeID { get; set; }

	public int? ParentOrderID { get; set; } = null;

	[Range(1, 999)]
	public int? OrderNum { get; set; }

	[Range(0, DAY_IN_SECONDS)]
	public double? OrderDuration { get; set; } = null;

	[Range(0, DAY_IN_SECONDS)]
	public double? PrepDuration { get; set; } = null;

	[DateNotInFuture]
	[DataType(DataType.DateTime)]
	public DateTime Date { get; set; } = DateTime.Now;
	
	public bool IsClosed { get; set; } = false;
}