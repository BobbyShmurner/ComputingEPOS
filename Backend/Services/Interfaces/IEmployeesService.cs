using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface IEmployeesService {
	public Task<ActionResult<List<Employee>>> GetAllEmployees();
	public Task<ActionResult<Employee>> GetEmployee(int id);

    public Task<ActionResult<Employee>> PutEmployee(Employee employee);

	public Task<ActionResult<Employee>> PostEmployee(Employee employee);

	public Task<IActionResult> DeleteEmployee(int id);

	public Task<bool> EmployeeExists(int id);
}
