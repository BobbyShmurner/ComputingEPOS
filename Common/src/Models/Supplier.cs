using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Common.Models;

[Table("Suppliers")]
public class Supplier {
	[Key]
	public int SupplierID { get; set; }

	[Required]
	public string? Name { get; set; }

	[Required]
	public string? Address { get; set; }

	[Required]
	[EmailAddress]
	public string? Email { get; set; }

	[Phone]
	[Required]
	public string? Phone { get; set; }
}