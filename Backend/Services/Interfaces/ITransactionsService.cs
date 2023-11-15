using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public interface ITransactionsService {
	public Task<List<Transaction>> GetTransactions(int? orderID);
	public Task<Transaction?> GetTransaction(int id);

	public Task<Transaction?> PutTransaction(Transaction transaction);

	public Task<Transaction> PostTransaction(Transaction transaction);

	public Task<bool> DeleteTransaction(int id);

	public Task<bool> TransactionExists(int id);
}
