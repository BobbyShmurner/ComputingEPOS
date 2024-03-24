using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface IMenu_MenuItemsService {
	public Task<ActionResult<List<Menu_MenuItem>>> GetMenu_MenuItems();
	public Task<ActionResult<Menu_MenuItem>> GetMenu_MenuItem(int id);
	
	public Task<ActionResult<Menu_MenuItem>> PutMenu_MenuItem(Menu_MenuItem menu_MenuItem);
	
	public Task<ActionResult<Menu_MenuItem>> PostMenu_MenuItem(Menu_MenuItem menu_MenuItem);
	
	public Task<IActionResult> DeleteMenu_MenuItem(int id);
	
	public Task<bool> Menu_MenuItemExists(int id);
}