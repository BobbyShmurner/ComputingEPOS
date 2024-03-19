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

    public EmployeesController(IEmployeesService service) {
        m_Service = service;
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

    // PUT: api/Employees/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Employee>> PutEmployee(int id, Employee employee) {
        if (id != employee.EmployeeID) return BadRequest(); 
        return await m_Service.PutEmployee(employee);
    }

    // POST: api/Employees
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee)  {
        ActionResult<Employee> newEmployeeRes = await m_Service.PostEmployee(employee);
        if (newEmployeeRes.Result != null) return newEmployeeRes.Result;
        Employee newEmployee = newEmployeeRes.Value!;
        
        return CreatedAtAction(nameof(GetEmployee), new { id = newEmployee.EmployeeID }, newEmployee);
    }

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(int id) =>
        await m_Service.DeleteEmployee(id);
}
