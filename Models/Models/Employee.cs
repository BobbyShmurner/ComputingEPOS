using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Employees")]
public class Employee : ICopyable<Employee> {
	[Key]
	public int EmployeeID { get; set; }

	[Required]
	[RegularExpression(@"^[a-zA-Z ,.'-]+$")]
	public string? FirstNames { get; set; } = "John";

	[Required]
	[RegularExpression(@"^[a-zA-Z,.'-]+$")]
	public string? LastName { get; set; } = "Doe";

	[Required]
	[Range(16, 100)]
	public int Age { get; set; }

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

	[Required]
	public decimal Wage { get; set; }

    public Employee Copy() => new Employee {
        EmployeeID = EmployeeID,
        FirstNames = FirstNames,
        LastName = LastName,
		Age = Age,
        Role = Role,
        DateJoined = DateJoined,
        Email = Email,
        Mobile = Mobile,
		Wage = Wage,
    };

    public enum Roles {
		Cashier,
		Kitchen,
		Supervisor,
		Manager,
	}
}