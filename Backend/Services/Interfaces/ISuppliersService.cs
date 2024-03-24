using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface ISuppliersService {
	public Task<ActionResult<List<Supplier>>> GetSuppliers();
	public Task<ActionResult<Supplier>> GetSupplier(int id);
	
	public Task<ActionResult<Supplier>> PutSupplier(Supplier supplier);
	
	public Task<ActionResult<Supplier>> PostSupplier(Supplier supplier);
	
	public Task<IActionResult> DeleteSupplier(int id);
	
	public Task<bool> SupplierExists(int id);
}
