using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputingEPOS.Models;

[Table("MenuButtons")]
public class MenuButton {
	[Key]
	public int MenuButtonID { get; set; }

	[Required]
	public int MenuID { get; set; }

	[Required]
	[EnumDataType(typeof(MenuButtonTypes))]
	public string? ButtonType { get; set; } = MenuButtonTypes.Basic.ToString();

	[Required]
	public string JsonData { get; set; } = "{}";

	public enum MenuButtonTypes {
		Basic,
		SubItem
	}
}