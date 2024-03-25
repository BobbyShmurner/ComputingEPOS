using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("MenuItems")]
public class MenuItem : ICopyable<MenuItem> {
	[Key]
	public int MenuItemID { get; set; }

	[Required]
	public int StockID { get; set; }

	public string? Note { get; set; }

	public decimal? Price { get; set; }

    public MenuItem Copy() => new MenuItem {
		MenuItemID = MenuItemID,
		StockID = StockID,
		Price = Price,
		Note = Note
	};
}