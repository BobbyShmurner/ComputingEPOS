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

public class EmployeesService : IEmployeesService {
	private readonly BaseDbContext _context;

	public EmployeesService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<Employee>>> GetAllEmployees() => 
		await _context.Employees.ToListAsync();

	public async Task<ActionResult<Employee>> GetEmployee(int id) {
        Employee? Employee = await _context.Employees.FindAsync(id);
		return Employee != null ? Employee : new NotFoundResult();
	}

	public ActionResult<Employee> GetEmployeeFromPin(string pin, IHashService hashService) {
		foreach (var e in _context.Employees.Where(e => e.PinHash != null)) {
			byte[] hash = GeneratePinHash(e, pin, hashService);
			if (hash.SequenceEqual(Convert.FromBase64String(e.PinHash!))) return e;
		};

		return new NotFoundResult();
	}

	public async Task<ActionResult<bool>> IsPinCorrect(int id, string pin, IHashService hashService) {
		ActionResult<Employee> employeeRes = await GetEmployee(id);
		if (employeeRes.Result != null) return employeeRes.Result;
		Employee employee = employeeRes.Value!;

		return IsPinCorrect(employee, pin, hashService);
	}

	bool IsPinCorrect(Employee employee, string pin, IHashService hashService) {
		if (employee.PinHash == null) return false;
		byte[] hash = GeneratePinHash(employee, pin, hashService);
		return hash.SequenceEqual(Convert.FromBase64String(employee.PinHash));
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

	ActionResult<Employee> SetPinHash(Employee employee, string pin, IHashService hashService) {
		if (PinExist(pin, hashService)) return new BadRequestObjectResult(new {detail = "Pin already exists!"});
		employee.PinHash = Convert.ToBase64String(GeneratePinHash(employee, pin, hashService));
		return employee;
	}

	public async Task<ActionResult<Employee>> PostEmployee(Employee employee, string? pin, IHashService hashService) {
		if (pin != null) {
			ActionResult<Employee> employeeRes = SetPinHash(employee, pin, hashService);
			if (employeeRes.Result != null) return employeeRes.Result;
			employee = employeeRes.Value!;
		}

		_context.Employees.Add(employee);
		await _context.SaveChangesAsync();
		return employee;
	}

	public async Task<ActionResult<Employee>> UpdatePin(int id, string accessPin, string newPin, IHashService hashService) {
		ActionResult<Employee> employeeRes = await GetEmployee(id);
		if (employeeRes.Result != null) return employeeRes.Result;
		Employee employee = employeeRes.Value!;

		if (!IsPinCorrect(employee, accessPin, hashService)) {
			// If the pin is a manager's pin, allow the update
			if (!IsManagerPin(accessPin, hashService)) return new UnauthorizedResult();
		}

		employeeRes = SetPinHash(employee, newPin, hashService);
		if (employeeRes.Result != null) return employeeRes.Result;
		employee = employeeRes.Value!;

		return await PutEmployee(employee);
	}

	public async Task<IActionResult> DeleteEmployee(int id) {
		ActionResult<Employee> employeeRes = await GetEmployee(id);
		if (employeeRes.Result != null) return employeeRes.Result;

		_context.Employees.Remove(employeeRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<ActionResult<string>> GeneratePinHash(int id, string pin, IHashService hashService) {
		ActionResult<Employee> employeeRes = await GetEmployee(id);
		if (employeeRes.Result != null) return employeeRes.Result;
		Employee employee = employeeRes.Value!;

		return Convert.ToBase64String(GeneratePinHash(employee, pin, hashService));
	}

	byte[] GeneratePinHash(Employee employee, string pin, IHashService hashService) {
		byte[] salt = (employee.FirstNames + employee.LastName).Select(c => (byte)c).ToArray();
		return hashService.GenerateSaltedHash(pin, salt);
	}

	public async Task<bool> EmployeeExists(int id) => 
		await _context.Employees.AnyAsync(e => e.EmployeeID == id);

	public bool PinExist(string pin, IHashService hashService) {
		foreach (var e in _context.Employees.Where(e => e.PinHash != null)) {
			byte[] hash = GeneratePinHash(e, pin, hashService);
			if (hash.SequenceEqual(Convert.FromBase64String(e.PinHash!))) return true;
		};

		return false;
	}

	public bool IsManagerPin(string pin, IHashService hashService) {
		foreach (var e in _context.Employees.Where(e => e.Role == Employee.Roles.Manager && e.PinHash != null)) {
			byte[] hash = GeneratePinHash(e, pin, hashService);
			if (hash.SequenceEqual(Convert.FromBase64String(e.PinHash!))) return true;
		};

		return false;
	}
}
