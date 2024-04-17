using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase {
    private readonly ITransactionsService m_Service;
    private readonly IOrdersService m_OrdersService;

    public TransactionsController(ITransactionsService service, IOrdersService ordersService) {
        m_Service = service;
        m_OrdersService = ordersService;
    }

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

    // GET: api/Transactions/GrossSales
    [HttpGet("GrossSales")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<decimal>> GetGrossSales(DateTime? from, DateTime? to) =>
        await m_Service.GetGrossSales(from, to);

    // GET: api/Transactions/GrossSalesInIntervals
    [HttpGet("GrossSalesInIntervals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<decimal>>> GetGrossSalesInIntervals(DateTime from, DateTime? to, long intervalInSeconds) =>
        await m_Service.GetGrossSalesInIntervals(from, to, intervalInSeconds);

    // PUT: api/Transactions/5
    /*[HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction>> PutTransaction(int id, Transaction transaction) {
        if (id != transaction.TransactionID) return BadRequest(); 
        return await m_Service.PutTransaction(transaction);
    }*/

    // POST: api/Transactions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)  {
        ActionResult<Transaction> newTransactionRes = await m_Service.PostTransaction(transaction, m_OrdersService);
        if (newTransactionRes.Result != null) return newTransactionRes.Result;
        Transaction newTransaction = newTransactionRes.Value!;
        
        return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.TransactionID }, newTransaction);
    }

    // DELETE: api/Transactions/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransaction(int id) =>
        await m_Service.DeleteTransaction(id, m_OrdersService);

    // DELETE: api/Transactions/DeleteDangling
    [HttpDelete("DeleteDangling")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDanglingTransactions() =>
        await m_Service.DeleteDanglingTransactions(m_OrdersService);
}
