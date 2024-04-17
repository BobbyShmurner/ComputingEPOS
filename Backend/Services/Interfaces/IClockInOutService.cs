using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public interface IClockInOutService {
	public Task<ActionResult<List<ClockInOut>>> GetClockInOut();
	public Task<ActionResult<ClockInOut>> GetClockInOut(int id);
	
	public Task<ActionResult<ClockInOut>> PutClockInOut(ClockInOut clockInOut);
	
	public Task<ActionResult<ClockInOut>> PostClockInOut(ClockInOut clockInOut);
	
	public Task<IActionResult> DeleteClockInOut(int id);
	
	public Task<bool> ClockInOutExists(int id);
}
