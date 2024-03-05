using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface ITransactionsService {
	public Task<ActionResult<List<Transaction>>> GetTransactions(int? orderID);
	public Task<ActionResult<Transaction>> GetTransaction(int id);
	public Task<ActionResult<decimal>> GetGrossSales(DateTime? from, DateTime? to);
	public Task<ActionResult<List<decimal>>> GetGrossSalesInIntervals(DateTime from, DateTime? to, long intervalInSeconds);

    public Task<ActionResult<Transaction>> PutTransaction(Transaction transaction);

	public Task<ActionResult<Transaction>> PostTransaction(Transaction transaction);

	public Task<IActionResult> DeleteTransaction(int id);

	public Task<bool> TransactionExists(int id);
}
