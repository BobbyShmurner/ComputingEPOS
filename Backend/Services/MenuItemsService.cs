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

public class MenuItemsService : IMenuItemsService {
    private readonly BaseDbContext _context;

    public MenuItemsService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<MenuItem>>> GetMenuItems() =>
        await _context.MenuItems.ToListAsync();

    public async Task<ActionResult<MenuItem>> GetMenuItem(int id) {
        MenuItem? menuItem = await _context.MenuItems.FindAsync(id);
        return menuItem != null ? menuItem : new NotFoundResult();
    }

    public async Task<ActionResult<MenuItem>> PutMenuItem(MenuItem menuItem) {
        _context.Entry(menuItem).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await MenuItemExists(menuItem.MenuItemID)) return new NotFoundResult();
            else throw;
        }

        return menuItem;
    }

    public async Task<ActionResult<MenuItem>> PostMenuItem(MenuItem menuItem) {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<IActionResult> DeleteMenuItem(int id) {
        ActionResult<MenuItem> menuItemRes = await GetMenuItem(id);
        if (menuItemRes.Result != null) return menuItemRes.Result;

        _context.MenuItems.Remove(menuItemRes.Value!);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> MenuItemExists(int id) => 
        await _context.MenuItems.AnyAsync(e => e.MenuItemID == id);
}