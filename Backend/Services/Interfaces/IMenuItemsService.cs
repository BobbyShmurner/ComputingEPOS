using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public interface IMenuItemsService {
	public Task<ActionResult<List<MenuItem>>> GetMenuItems();
	public Task<ActionResult<MenuItem>> GetMenuItem(int id);
	
	public Task<ActionResult<MenuItem>> PutMenuItem(MenuItem menuItem);
	
	public Task<ActionResult<MenuItem>> PostMenuItem(MenuItem menuItem);
	
	public Task<IActionResult> DeleteMenuItem(int id);
	
	public Task<bool> MenuItemExists(int id);
}