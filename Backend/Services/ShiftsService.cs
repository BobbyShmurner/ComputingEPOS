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

public class ShiftsService : IShiftsService {
	private readonly BaseDbContext _context;

	public ShiftsService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<Shift>>> GetShifts() =>
		await _context.Shifts.ToListAsync();

	public async Task<ActionResult<Shift>> GetShift(int id) {
		Shift? shift = await _context.Shifts.FindAsync(id);
		return shift != null ? shift : new NotFoundResult();
	}

	public async Task<ActionResult<Shift>> PutShift(Shift shift) {
		_context.Entry(shift).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await ShiftExists(shift.ShiftID)) return new NotFoundResult();
			else throw;
		}

		return shift;
	}

	public async Task<ActionResult<Shift>> PostShift(Shift shift) {
		_context.Shifts.Add(shift);
		await _context.SaveChangesAsync();
		return shift;
	}

	public async Task<IActionResult> DeleteShift(int id) {
		ActionResult<Shift> shiftRes = await GetShift(id);
		if (shiftRes.Result != null) return shiftRes.Result;

		_context.Shifts.Remove(shiftRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<bool> ShiftExists(int id) => 
		await _context.Shifts.AnyAsync(e => e.ShiftID == id);
}
