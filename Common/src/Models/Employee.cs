using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Common.Models;

[Table("Employees")]
public class Employee : ICopyable<Employee> {
	/// <summary>
	/// The ID of the employee record.
	/// </summary>
	[Key]
	public int EmployeeID { get; set; }

	/// <summary>
	/// The First Name(s) of the employee.
	/// Can only contain letters A-Z, spaces, commas, dots, apostrophes and dashes.
	/// </summary>
	[Required]
	[RegularExpression(@"^[a-zA-Z ,.'-]+$")]
	public string FirstNames { get; set; } = "John";

	/// <summary>
	/// The surname of the employee.
	/// Can only contain letters A-Z, commas, dots, apostrophes and dashes.
	/// </summary>
	[Required]
	[RegularExpression(@"^[a-zA-Z,.'-]+$")]
	public string LastName { get; set; } = "Doe";

	/// <summary>
	/// The age of the employee. Must be between 16 and 100
	/// </summary>
	[Required]
	[Range(16, 100)]
	public int Age { get; set; }

	/// <summary>
	/// The Role of the employee.
	/// </summary>
	[Required]
	public Roles Role { get; set; } = Roles.Cashier;

	/// <summary>
	/// The date the employee was created.
	/// </summary>
	[DataType(DataType.DateTime)]
	public DateTime DateJoined { get; set; } = DateTime.Now;

	/// <summary>
	/// The email of the employee.
	/// </summary>
	[Required]
	[EmailAddress]
	public string? Email { get; set; } = "john.doe@email.com";

	/// <summary>
	/// The mobile of the employee.
	/// </summary>
	[Phone]
	[Required]
	public string? Mobile { get; set; } = "+44 7777777777";

	/// <summary>
	/// The Wage of the employee.
	/// </summary>
	[Required]
	public decimal Wage { get; set; }

	/// <summary>
	/// The has of the employee's pin.
	/// Can be null if the employee hasn't yet been assigned a pin.
	/// </summary>
	public string? PinHash { get; set; }

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
		PinHash = PinHash,
    };

	/// <summary>
	/// The Roles an employee can be assigned.
	/// </summary>
    public enum Roles {
		Cashier = 0,
		Kitchen = 1,
		Supervisor = 2,
		Manager = 3,
	}
}