using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Transactions")]
public class Transaction {
	[Key]
	public int TransactionID { get; set; }

	[Required]
	public int OrderID { get; set; }

	[Required]
	public decimal AmountPaid { get; set; }

	[Required]
	[EnumDataType(typeof(PaymentMethods))]
	public string? Method { get; set; }

	[DataType(DataType.DateTime)]
	public DateTime Date { get; set; } = DateTime.Now;

	public enum PaymentMethods {
		Cash,
		Card
	}
}