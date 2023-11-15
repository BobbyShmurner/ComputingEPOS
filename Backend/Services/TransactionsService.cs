using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ComputingEPOS.Backend.Services;

public class TransactionsService : ITransactionsService {
    private readonly BaseDbContext _context;

    public TransactionsService(BaseDbContext context) =>  _context = context;

    public async Task<List<Transaction>> GetTransactions(int? orderId) {
        if (orderId == null) return await _context.Transactions.ToListAsync();
        return await _context.Transactions.Where(x => x.OrderID == orderId).ToListAsync();
    }

    public async Task<Transaction?> GetTransaction(int id) 
        => await _context.Transactions.FindAsync(id);

    public async Task<Transaction?> PutTransaction(Transaction transaction) {
        _context.Entry(transaction).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await TransactionExists(transaction.TransactionID)) return null;
            else throw;
        }

        return transaction;
    }

    public async Task<Transaction> PostTransaction(Transaction transaction)  {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<bool> DeleteTransaction(int id) {
        Transaction? transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TransactionExists(int id) => 
        await _context.Transactions.AnyAsync(e => e.TransactionID == id);
}
