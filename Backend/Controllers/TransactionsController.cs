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
public class TransactionsController : ControllerBase {
    private readonly ITransactionsService m_Service;

    public TransactionsController(ITransactionsService service) =>
         m_Service = service;

    // GET: api/Transactions
    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Transaction>>> GetTransactions(int? orderId = null) =>
        await m_Service.GetTransactions(orderId);

    // GET: api/Transactions/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Transaction>> GetTransaction(int id) =>
        await m_Service.GetTransaction(id);

    // PUT: api/Transactions/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction>> PutTransaction(int id, Transaction transaction) {
        if (id != transaction.TransactionID) return BadRequest(); 
        return await m_Service.PutTransaction(transaction);
    }

    // POST: api/Transactions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)  {
        ActionResult<Transaction> newTransactionRes = await m_Service.PostTransaction(transaction);
        if (newTransactionRes.Result != null) return newTransactionRes.Result;
        Transaction newTransaction = newTransactionRes.Value!;
        
        return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.TransactionID }, newTransaction);
    }

    // DELETE: api/Transactions/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransaction(int id) =>
        await m_Service.DeleteTransaction(id);
}
