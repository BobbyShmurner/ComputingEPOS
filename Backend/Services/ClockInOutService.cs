using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public class ClockInOutService : IClockInOutService {
	private readonly BaseDbContext _context;

	public ClockInOutService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<ClockInOut>>> GetClockInOut() =>
		await _context.ClockInOut.ToListAsync();

	public async Task<ActionResult<ClockInOut>> GetClockInOut(int id) {
		ClockInOut? clockInOut = await _context.ClockInOut.FindAsync(id);
		return clockInOut != null ? clockInOut : new NotFoundResult();
	}

	public async Task<ActionResult<ClockInOut>> PutClockInOut(ClockInOut clockInOut) {
		_context.Entry(clockInOut).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await ClockInOutExists(clockInOut.ClockInOutID)) return new NotFoundResult();
			else throw;
		}

		return clockInOut;
	}

	public async Task<ActionResult<ClockInOut>> PostClockInOut(ClockInOut clockInOut) {
		_context.ClockInOut.Add(clockInOut);
		await _context.SaveChangesAsync();
		return clockInOut;
	}

	public async Task<IActionResult> DeleteClockInOut(int id) {
		ActionResult<ClockInOut> clockInOutRes = await GetClockInOut(id);
		if (clockInOutRes.Result != null) return clockInOutRes.Result;

		_context.ClockInOut.Remove(clockInOutRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<bool> ClockInOutExists(int id) => 
		await _context.ClockInOut.AnyAsync(e => e.ClockInOutID == id);
}
