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
public class MenusController : ControllerBase {
    private readonly IMenusService m_Service;

    public MenusController(IMenusService service) =>
         m_Service = service;

    // GET: api/Menus
    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Menu>>> GetMenus(bool? visible) =>
        await m_Service.GetMenus(visible);

    // GET: api/Menus/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Menu>> GetMenu(int id) =>
        await m_Service.GetMenu(id);

    // PUT: api/Menus/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Menu>> PutMenu(int id, Menu menu) {
        if (id != menu.MenuID) return BadRequest(); 
        return await m_Service.PutMenu(menu);
    }

    // POST: api/Menus
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Menu>> PostMenu(Menu menu)  {
        ActionResult<Menu> newMenuRes = await m_Service.PostMenu(menu);
        if (newMenuRes.Result != null) return newMenuRes.Result;
        Menu newMenu = newMenuRes.Value!;
        
        return CreatedAtAction(nameof(GetMenu), new { id = newMenu.MenuID }, newMenu);
    }

    // DELETE: api/Menus/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenu(int id) =>
        await m_Service.DeleteMenu(id);
}
