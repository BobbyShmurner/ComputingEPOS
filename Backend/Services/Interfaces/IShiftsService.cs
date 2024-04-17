using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public interface IShiftsService {
	public Task<ActionResult<List<Shift>>> GetShifts();
	public Task<ActionResult<Shift>> GetShift(int id);
	
	public Task<ActionResult<Shift>> PutShift(Shift shift);
	
	public Task<ActionResult<Shift>> PostShift(Shift shift);
	
	public Task<IActionResult> DeleteShift(int id);
	
	public Task<bool> ShiftExists(int id);
}
