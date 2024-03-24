using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ComputingEPOS.Backend.Services;

public class Menu_MenuItemsService : IMenu_MenuItemsService {
    private readonly BaseDbContext _context;

    public Menu_MenuItemsService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<Menu_MenuItem>>> GetMenu_MenuItems() =>
        await _context.Menu_MenuItems.ToListAsync();

    public async Task<ActionResult<Menu_MenuItem>> GetMenu_MenuItem(int id) {
        Menu_MenuItem? menu_MenuItems = await _context.Menu_MenuItems.FindAsync(id);
        return menu_MenuItems != null ? menu_MenuItems : new NotFoundResult();
    }

    public async Task<ActionResult<Menu_MenuItem>> PutMenu_MenuItem(Menu_MenuItem menu_MenuItems) {
        _context.Entry(menu_MenuItems).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await Menu_MenuItemExists(menu_MenuItems.Menu_MenuItemID)) return new NotFoundResult();
            else throw;
        }

        return menu_MenuItems;
    }

    public async Task<ActionResult<Menu_MenuItem>> PostMenu_MenuItem(Menu_MenuItem menu_MenuItems) {
        _context.Menu_MenuItems.Add(menu_MenuItems);
        await _context.SaveChangesAsync();
        return menu_MenuItems;
    }

    public async Task<IActionResult> DeleteMenu_MenuItem(int id) {
        ActionResult<Menu_MenuItem> menu_MenuItemsRes = await GetMenu_MenuItem(id);
        if (menu_MenuItemsRes.Result != null) return menu_MenuItemsRes.Result;

        _context.Menu_MenuItems.Remove(menu_MenuItemsRes.Value!);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> Menu_MenuItemExists(int id) => 
        await _context.Menu_MenuItems.AnyAsync(e => e.Menu_MenuItemID == id);
}