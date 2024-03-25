using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Menus")]
public class Menu : ICopyable<Menu> {
	[Key]
	public int MenuID { get; set; }

	[Required]
	public string Name { get; set; } = "Menu";

	[DataType(DataType.DateTime)]
	public DateTime Date { get; set; } = DateTime.Now;

    public int? Rows { get; set; }
	public int? Columns { get; set; }

	public bool Visible { get; set; } = true;

	public Menu Copy() => new Menu {
        MenuID = MenuID,
        Name = Name,
        Date = Date,
        Rows = Rows,
        Columns = Columns,
		Visible = Visible,
    };
}