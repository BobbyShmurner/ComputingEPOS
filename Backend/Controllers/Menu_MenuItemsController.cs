using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Menu_MenuItemsController : ControllerBase {
	private readonly IMenu_MenuItemsService m_Service;
	
	public Menu_MenuItemsController(IMenu_MenuItemsService service) =>
		m_Service = service;
	
	// GET: api/Menu_MenuItems
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<Menu_MenuItem>>> GetMenu_MenuItems() =>
		await m_Service.GetMenu_MenuItems();
	
	// GET: api/Menu_MenuItems/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Menu_MenuItem>> GetMenu_MenuItem(int id) =>
		await m_Service.GetMenu_MenuItem(id);
	
	// PUT: api/Menu_MenuItems/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Menu_MenuItem>> PutMenu_MenuItem(int id, Menu_MenuItem menu_MenuItem) {
		if (id != menu_MenuItem.Menu_MenuItemID) return BadRequest(); 
		return await m_Service.PutMenu_MenuItem(menu_MenuItem);
	}
	
	// POST: api/Menu_MenuItems
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<ActionResult<Menu_MenuItem>> PostMenu_MenuItem(Menu_MenuItem menu_MenuItem)  {
		ActionResult<Menu_MenuItem> newMenu_MenuItemRes = await m_Service.PostMenu_MenuItem(menu_MenuItem);
		if (newMenu_MenuItemRes.Result != null) return newMenu_MenuItemRes.Result;
		Menu_MenuItem newMenu_MenuItem = newMenu_MenuItemRes.Value!;
		
		return CreatedAtAction(nameof(GetMenu_MenuItem), new { id = newMenu_MenuItem.Menu_MenuItemID }, newMenu_MenuItem);
	}
	
	// DELETE: api/Menu_MenuItems/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteMenu_MenuItem(int id) =>
		await m_Service.DeleteMenu_MenuItem(id);
}