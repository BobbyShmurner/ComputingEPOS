using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShiftsController : ControllerBase {
	private readonly IShiftsService m_Service;
	
	public ShiftsController(IShiftsService service) =>
		m_Service = service;
	
	// GET: api/Shifts
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<Shift>>> GetShifts() =>
		await m_Service.GetShifts();
	
	// GET: api/Shifts/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Shift>> GetShift(int id) =>
		await m_Service.GetShift(id);
	
	// PUT: api/Shifts/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Shift>> PutShift(int id, Shift shift) {
		if (id != shift.ShiftID) return BadRequest(); 
		return await m_Service.PutShift(shift);
	}
	
	// POST: api/Shifts
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<ActionResult<Shift>> PostShift(Shift shift)  {
		ActionResult<Shift> newShiftRes = await m_Service.PostShift(shift);
		if (newShiftRes.Result != null) return newShiftRes.Result;
		Shift newShift = newShiftRes.Value!;
		
		return CreatedAtAction(nameof(GetShift), new { id = newShift.ShiftID }, newShift);
	}
	
	// DELETE: api/Shifts/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteShift(int id) =>
		await m_Service.DeleteShift(id);
}
