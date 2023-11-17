using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Menus")]
public class Menu {
	[Key]
	public int MenuID { get; set; }

	[Required]
	public string JsonData { get; set; } = "{}";

	[DataType(DataType.DateTime)]
	public DateTime Date { get; set; }
}