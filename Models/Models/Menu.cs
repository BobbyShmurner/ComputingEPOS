using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Menus")]
public class Menu {
	[Key]
	public int MenuID { get; set; }

	[Required]
	public string Name { get; set; } = "Menu";

	[DataType(DataType.DateTime)]
	public DateTime Date { get; set; }

	public int? Rows { get; set; }
	public int? Columns { get; set; }
}