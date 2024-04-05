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
public class EmployeesController : ControllerBase {
    private readonly IEmployeesService m_Service;
    private readonly IHashService m_HashService;

    public EmployeesController(IEmployeesService service, IHashService hashService) {
        m_Service = service;
        m_HashService = hashService;
    }

    // GET: api/Employees
    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Employee>>> GetEmployees() =>
        await m_Service.GetAllEmployees();

    // GET: api/Employees/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Employee>> GetEmployee(int id) =>
        await m_Service.GetEmployee(id);

    // GET: api/Employees/FromPin?pin=1234
    [HttpGet("FromPin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Employee> GetEmployeeFromPin([Required] string pin) =>
        m_Service.GetEmployeeFromPin(pin, m_HashService);

    // GET: api/Employees/5/IsPinCorrect?pin=1234
    [HttpGet("{id}/IsPinCorrect")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> IsPinCorrect(int id, [Required] string pin) =>
        await m_Service.IsPinCorrect(id, pin, m_HashService);

    // GET api/Employees/5/GeneratePinHash?pin=1234
    [HttpGet("{id}/GeneratePinHash")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GeneratePinHash(int id, [Required] string pin) =>
        await m_Service.GeneratePinHash(id, pin, m_HashService);

    // GET: api/PinExists?pin=1234
    [HttpGet("PinExists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> PinExist([Required] string pin) =>
        m_Service.PinExist(pin, m_HashService);

    // PUT: api/Employees/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Employee>> PutEmployee(int id, Employee employee) {
        if (id != employee.EmployeeID) return BadRequest(); 
        return await m_Service.PutEmployee(employee);
    }

    // POST: api/Employees?pin=1234
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee, [Required] string pin)  {
        ActionResult<Employee> newEmployeeRes = await m_Service.PostEmployee(employee, pin, m_HashService);
        if (newEmployeeRes.Result != null) return newEmployeeRes.Result;
        Employee newEmployee = newEmployeeRes.Value!;
        
        return CreatedAtAction(nameof(GetEmployee), new { id = newEmployee.EmployeeID }, newEmployee);
    }

    // POST: api/Employees/5/UpdatePin?accessPin=1234&newPin=5678
    [HttpPost("{id}/UpdatePin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Employee>> UpdatePin(int id, [Required] string accessPin, [Required] string newPin) =>
        await m_Service.UpdatePin(id, accessPin, newPin, m_HashService);

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(int id) =>
        await m_Service.DeleteEmployee(id);
}
