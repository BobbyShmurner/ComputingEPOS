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
public class SuppliersController : ControllerBase {
	private readonly ISuppliersService m_Service;
	
	public SuppliersController(ISuppliersService service) =>
		m_Service = service;
	
	// GET: api/Suppliers
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<Supplier>>> GetSuppliers() =>
		await m_Service.GetSuppliers();
	
	// GET: api/Suppliers/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Supplier>> GetSupplier(int id) =>
		await m_Service.GetSupplier(id);
	
	// PUT: api/Suppliers/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Supplier>> PutSupplier(int id, Supplier supplier) {
		if (id != supplier.SupplierID) return BadRequest(); 
		return await m_Service.PutSupplier(supplier);
	}
	
	// POST: api/Suppliers
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)  {
		ActionResult<Supplier> newSupplierRes = await m_Service.PostSupplier(supplier);
		if (newSupplierRes.Result != null) return newSupplierRes.Result;
		Supplier newSupplier = newSupplierRes.Value!;
		
		return CreatedAtAction(nameof(GetSupplier), new { id = newSupplier.SupplierID }, newSupplier);
	}
	
	// DELETE: api/Suppliers/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteSupplier(int id) =>
		await m_Service.DeleteSupplier(id);
}
