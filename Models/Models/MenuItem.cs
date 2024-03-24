using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("MenuItems")]
public class MenuItem {
	[Key]
	public int MenuItemID { get; set; }

	[Required]
	public string DisplayText { get; set; } = "Menu Item";

	public int? StockID { get; set; }

	public double? Price { get; set; }
}