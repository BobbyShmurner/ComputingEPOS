using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Backend.Models;

[Table("Employees")]
public class Employee {
	[Key]
	public int EmployeeID { get; set; }

	[Required]
	public int WageID { get; set; }

	[Required]
	[RegularExpression(@"^[a-zA-Z ,.'-]+$")]
	public string? FirstNames { get; set; } = "John";

	[Required]
	[RegularExpression(@"^[a-zA-Z,.'-]+$")]
	public string? LastName { get; set; } = "Doe";

	[Required]
	[EnumDataType(typeof(Roles))]
	public string? Role { get; set; } = Roles.Cashier.ToString();

	[DataType(DataType.DateTime)]
	public DateTime DateJoined { get; set; } = DateTime.Now;

	[Required]
	[EmailAddress]
	public string? Email { get; set; } = "john.doe@email.com";

	[Phone]
	[Required]
	public string? Mobile { get; set; } = "+44 7777777777";

	public enum Roles {
		Cashier,
		Kitchen,
		Supervisor,
		Manager,
	}
}