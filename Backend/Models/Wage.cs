using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Backend.Models;

[Table("Wages")]
public class Wage {
	[Key]
	public int WageID { get; set; }

	[Required]
	public decimal BaseRate { get; set; }

	public decimal? OvertimeRate { get; set; } = null;

	public decimal? ContractedHours { get; set; } = null;
}