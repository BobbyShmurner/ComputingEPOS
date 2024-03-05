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

public class TransactionsService : ITransactionsService {
    private readonly BaseDbContext _context;

    public TransactionsService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<Transaction>>> GetTransactions(int? orderId) {
        if (orderId == null) return await _context.Transactions.ToListAsync();
        return await _context.Transactions.Where(x => x.OrderID == orderId).ToListAsync();
    }

    public async Task<ActionResult<Transaction>> GetTransaction(int id) {
        Transaction? transaction = await _context.Transactions.FindAsync(id);
        return transaction != null ? transaction : new NotFoundResult();
    }

    public async Task<ActionResult<decimal>> GetGrossSales(DateTime? from, DateTime? to)
    {
        try
        {
            return (await _context.Transactions.Where(x => (from == null || x.Date >= from) && (to == null || x.Date <= to)).ToListAsync())
                .Aggregate(new decimal(), (acc, x) => acc + x.AmountPaid);
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return new ProblemResult();
        }
    }

    public async Task<ActionResult<List<decimal>>> GetGrossSalesInIntervals(DateTime from, DateTime? to, long intervalInSeconds) {
        List<decimal> sales = new();
        long intervalInTicks = intervalInSeconds * 10000000;

        for (long i = from.Ticks; i <= (to?.Ticks ?? DateTime.Now.Ticks); i += intervalInTicks)
        {
            ActionResult<decimal> salesRes = await GetGrossSales(new DateTime(i), new DateTime(i + intervalInTicks));
            if (salesRes.Result != null) return salesRes.Result!;
            sales.Add(salesRes.Value!);
        }

        return sales;
    }

    public async Task<ActionResult<Transaction>> PutTransaction(Transaction transaction) {
        _context.Entry(transaction).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await TransactionExists(transaction.TransactionID)) return new NotFoundResult();
            else throw;
        }

        return transaction;
    }

    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction) {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<IActionResult> DeleteTransaction(int id) {
        ActionResult<Transaction> transactionRes = await GetTransaction(id);
        if (transactionRes.Result != null) return transactionRes.Result;

        _context.Transactions.Remove(transactionRes.Value!);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> TransactionExists(int id) => 
        await _context.Transactions.AnyAsync(e => e.TransactionID == id);
}
