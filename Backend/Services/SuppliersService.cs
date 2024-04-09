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

public class SuppliersService : ISuppliersService {
	private readonly BaseDbContext _context;

	public SuppliersService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<Supplier>>> GetSuppliers() =>
		await _context.Suppliers.ToListAsync();

	public async Task<ActionResult<Supplier>> GetSupplier(int id) {
		Supplier? supplier = await _context.Suppliers.FindAsync(id);
		return supplier != null ? supplier : new NotFoundResult();
	}

	public async Task<ActionResult<Supplier>> PutSupplier(Supplier supplier) {
		_context.Entry(supplier).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await SupplierExists(supplier.SupplierID)) return new NotFoundResult();
			else throw;
		}

		return supplier;
	}

	public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier) {
		_context.Suppliers.Add(supplier);
		await _context.SaveChangesAsync();
		return supplier;
	}

	public async Task<IActionResult> DeleteSupplier(int id) {
		ActionResult<Supplier> supplierRes = await GetSupplier(id);
		if (supplierRes.Result != null) return supplierRes.Result;

		_context.Suppliers.Remove(supplierRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<bool> SupplierExists(int id) => 
		await _context.Suppliers.AnyAsync(e => e.SupplierID == id);
}
