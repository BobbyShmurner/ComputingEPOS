using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using ComputingEPOS.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuItemsController : ControllerBase {
    private readonly IMenuItemsService m_Service;
    
    public MenuItemsController(IMenuItemsService service) {
        m_Service = service;
    }
    
    // GET: api/MenuItems
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MenuItem>>> GetMenuItems() =>
        await m_Service.GetMenuItems();
    
    // GET: api/MenuItems/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuItem>> GetMenuItem(int id) =>
        await m_Service.GetMenuItem(id);
    
    // PUT: api/MenuItems/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MenuItem>> PutMenuItem(int id, MenuItem menuItem) {
        if (id != menuItem.MenuItemID) return BadRequest(); 
        return await m_Service.PutMenuItem(menuItem);
    }
    
    // POST: api/MenuItems
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<MenuItem>> PostMenuItem(MenuItem menuItem)  {
        ActionResult<MenuItem> newMenuItemRes = await m_Service.PostMenuItem(menuItem);
        if (newMenuItemRes.Result != null) return newMenuItemRes.Result;
        MenuItem newMenuItem = newMenuItemRes.Value!;
        
        return CreatedAtAction(nameof(GetMenuItem), new { id = newMenuItem.MenuItemID }, newMenuItem);
    }
    
    // DELETE: api/MenuItems/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItem(int id) =>
        await m_Service.DeleteMenuItem(id);
}
