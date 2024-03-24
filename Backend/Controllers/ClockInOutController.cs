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
public class ClockInOutController : ControllerBase {
	private readonly IClockInOutService m_Service;
	
	public ClockInOutController(IClockInOutService service) =>
		m_Service = service;
	
	// GET: api/ClockInOut
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<ClockInOut>>> GetClockInOut() =>
		await m_Service.GetClockInOut();
	
	// GET: api/ClockInOut/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ClockInOut>> GetClockInOut(int id) =>
		await m_Service.GetClockInOut(id);
	
	// PUT: api/ClockInOut/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<ClockInOut>> PutClockInOut(int id, ClockInOut clockInOut) {
		if (id != clockInOut.ClockInOutID) return BadRequest(); 
		return await m_Service.PutClockInOut(clockInOut);
	}
	
	// POST: api/ClockInOut
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<ActionResult<ClockInOut>> PostClockInOut(ClockInOut clockInOut)  {
		ActionResult<ClockInOut> newClockInOutRes = await m_Service.PostClockInOut(clockInOut);
		if (newClockInOutRes.Result != null) return newClockInOutRes.Result;
		ClockInOut newClockInOut = newClockInOutRes.Value!;
		
		return CreatedAtAction(nameof(GetClockInOut), new { id = newClockInOut.ClockInOutID }, newClockInOut);
	}
	
	// DELETE: api/ClockInOut/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteClockInOut(int id) =>
		await m_Service.DeleteClockInOut(id);
}
