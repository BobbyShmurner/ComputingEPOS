using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Common.Models;

[Table("ClockInOut")]
public class ClockInOut {
	[Key]
	public int ClockInOutID { get; set; }

	[Required]
	public int EmployeeID { get; set; }

	[DataType(DataType.DateTime)]
	public DateTime ClockInDate { get; set; } = DateTime.Now;

	[DataType(DataType.DateTime)]
	public DateTime? ClockOutDate { get; set; } = null;
}