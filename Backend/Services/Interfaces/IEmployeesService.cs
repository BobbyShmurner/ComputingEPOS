using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface IEmployeesService {
	public Task<ActionResult<List<Employee>>> GetAllEmployees();
	public Task<ActionResult<Employee>> GetEmployee(int id);
	public ActionResult<Employee> GetEmployeeFromPin(string pin, IHashService hashService);
	public Task<ActionResult<string>> GeneratePinHash(int id, string pin, IHashService hashService);

	public Task<ActionResult<bool>> IsPinCorrect(int id, string pin, IHashService hashService);
	public bool IsManagerPin(string pin, IHashService hashService);

    public Task<ActionResult<Employee>> PutEmployee(Employee employee);

	public Task<ActionResult<Employee>> PostEmployee(Employee employee, string pin, IHashService hashService);
	public Task<ActionResult<Employee>> UpdatePin(int id, string accessPin, string newPin, IHashService hashService);

	public Task<IActionResult> DeleteEmployee(int id);

	public Task<bool> EmployeeExists(int id);
	public bool PinExist(string pin, IHashService hashService);
}
