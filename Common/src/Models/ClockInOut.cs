using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Common.Models;

[Table("ClockInOut")]
public class ClockInOut {
	/// <summary>
	/// The ID of the clock in/out record.
	/// </summary>
	[Key]
	public int ClockInOutID { get; set; }

	/// <summary>
	/// The ID of the employee who clocked in/out.
	/// </summary>
	[Required]
	public int EmployeeID { get; set; }

	/// <summary>
	/// The date the employee clocked in.
	/// </summary>
	[DataType(DataType.DateTime)]
	public DateTime ClockInDate { get; set; } = DateTime.Now;

	/// <summary>
	/// The date the employee clocked out. Can be null if the employee has not clocked out yet.
	/// </summary>
	[DataType(DataType.DateTime)]
	public DateTime? ClockOutDate { get; set; } = null;
}