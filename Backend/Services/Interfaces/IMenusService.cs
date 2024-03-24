using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface IMenusService {
	public Task<ActionResult<List<Menu>>> GetMenus();
	public Task<ActionResult<Menu>> GetMenu(int id);

	public Task<ActionResult<Menu>> PutMenu(Menu menu);

	public Task<ActionResult<Menu>> PostMenu(Menu menu);
	// public Task<ActionResult<Menu_MenuItem>> SetMenuItem(int id, int menuItemID, int row, int column);

	public Task<IActionResult> DeleteMenu(int id);

	public Task<bool> MenuExists(int id);
}
