using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("Menu_MenuItems")]
public class Menu_MenuItem : ICopyable<Menu_MenuItem> {
	[Required] public int Menu_MenuItemID { get; set; }
	[Required] public int MenuID { get; set; }
	[Required] public int MenuItemID { get; set; }

	[Required] public int Row { get; set; }
	[Required] public int Column { get; set; }

	public Menu_MenuItem Copy() => new Menu_MenuItem {
        Menu_MenuItemID = Menu_MenuItemID,
        MenuID = MenuID,
        MenuItemID = MenuItemID,
        Row = Row,
        Column = Column
    };
}