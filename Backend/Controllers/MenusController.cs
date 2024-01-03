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
    public async Task<ActionResult<List<Menu>>> GetMenus() =>
        await m_Service.GetMenus();

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
    public async Task<ActionResult<Menu>> PutMenu(int id, Menu menus) {
        if (id != menus.MenuID) return BadRequest(); 
        return await m_Service.PutMenu(menus);
    }

    // POST: api/Menus
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Menu>> PostMenu(Menu menus)  {
        ActionResult<Menu> newMenuRes = await m_Service.PostMenu(menus);
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
