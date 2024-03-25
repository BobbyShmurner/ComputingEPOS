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

public class MenusService : IMenusService {
    private readonly BaseDbContext _context;

    public MenusService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<Menu>>> GetMenus(bool? visible) =>
        await _context.Menus.Where(m => visible == null || m.Visible == visible).ToListAsync();

    public async Task<ActionResult<Menu>> GetMenu(int id) {
        Menu? menu = await _context.Menus.FindAsync(id);
        return menu != null ? menu : new NotFoundResult();
    }

    public async Task<ActionResult<Menu>> PutMenu(Menu menu) {
        _context.Entry(menu).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await MenuExists(menu.MenuID)) return new NotFoundResult();
            else throw;
        }

        return menu;
    }

    public async Task<ActionResult<Menu>> PostMenu(Menu menu) {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task<IActionResult> DeleteMenu(int id) {
        ActionResult<Menu> menuRes = await GetMenu(id);
        if (menuRes.Result != null) return menuRes.Result;

        _context.Menus.Remove(menuRes.Value!);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> MenuExists(int id) => 
        await _context.Menus.AnyAsync(e => e.MenuID == id);
}
