using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ComputingEPOS.Backend.Results;

namespace ComputingEPOS.Backend.Services;

public class EmployeesService : IEmployeesService {
	private readonly BaseDbContext _context;

	public EmployeesService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<Employee>>> GetAllEmployees() => 
		await _context.Employees.ToListAsync();

	public async Task<ActionResult<Employee>> GetEmployee(int id) {
        Employee? Employee = await _context.Employees.FindAsync(id);
		return Employee != null ? Employee : new NotFoundResult();
	}

	public async Task<ActionResult<Employee>> PutEmployee(Employee employee) {
		_context.Entry(employee).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await EmployeeExists(employee.EmployeeID)) return new NotFoundResult();
			else throw;
		}

		return employee;
	}

	public async Task<ActionResult<Employee>> PostEmployee(Employee employee)  {
		_context.Employees.Add(employee);
		await _context.SaveChangesAsync();
		return employee;
	}

	public async Task<IActionResult> DeleteEmployee(int id) {
		ActionResult<Employee> employeeRes = await GetEmployee(id);
		if (employeeRes.Result != null) return employeeRes.Result;

		_context.Employees.Remove(employeeRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<bool> EmployeeExists(int id) => 
		await _context.Employees.AnyAsync(e => e.EmployeeID == id);
}
