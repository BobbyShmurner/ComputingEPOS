using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComputingEPOS.Backend.Models;

[Table("Shifts")]
public class Shift {
	[Key]
	public int ShiftID { get; set; }

	[Required]
	public int EmployeeID { get; set; }

	[Required]
	[DataType(DataType.DateTime)]
	public DateTime StartDate { get; set; }

	[Required]
	[DataType(DataType.DateTime)]
	public DateTime EndDate { get; set; }
}