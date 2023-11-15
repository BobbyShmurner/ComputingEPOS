using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;
using ComputingEPOS.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase {
    private readonly ITransactionsService m_Service;

    public TransactionsController(ITransactionsService service) =>
         m_Service = service;

    // GET: api/Transactions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int? orderId = null)
        => await m_Service.GetTransactions(orderId);

    // GET: api/Transactions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id) {
        Transaction? transaction = await m_Service.GetTransaction(id);
        return transaction != null ? transaction : NotFound();
    }

    // PUT: api/Transactions/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Transaction>> PutTransaction(int id, Transaction transaction) {
        if (id != transaction.TransactionID) return BadRequest(); 
        
        Transaction? updatedTransaction = await m_Service.PutTransaction(transaction);
        return updatedTransaction != null ? updatedTransaction : NotFound();
    }

    // POST: api/Transactions
    [HttpPost]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)  {
        Transaction? newTransaction = await m_Service.PostTransaction(transaction);
        return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.TransactionID }, newTransaction);
    }

    // DELETE: api/Transactions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id) {
        if (!await m_Service.DeleteTransaction(id)) return NotFound();
        return Ok();
    }
}
