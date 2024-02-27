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

public class StockService : IStockService {
	private readonly BaseDbContext _context;

	public StockService(BaseDbContext context) =>  _context = context;

	public async Task<ActionResult<List<Stock>>> GetAllStock() => 
		await _context.Stock.ToListAsync();

	public async Task<ActionResult<Stock>> GetStock(int id) {
		Stock? stock = await _context.Stock.FindAsync(id);
		return stock != null ? stock : new NotFoundResult();
	}

	public async Task<ActionResult<Stock>> PutStock(Stock stock) {
		_context.Entry(stock).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await StockExists(stock.StockID)) return new NotFoundResult();
			else throw;
		}

		return stock;
	}

	public async Task<ActionResult<Stock>> PostStock(Stock stock)  {
		_context.Stock.Add(stock);
		await _context.SaveChangesAsync();
		return stock;
	}

	public async Task<ActionResult<Stock>> UpdateStockQuantity(int id, float delta) {
		ActionResult<Stock> stockRes = await GetStock(id);
		if (stockRes.Result != null) return stockRes.Result;
		Stock stock = stockRes.Value!;

		if (stock.Quantity + delta < 0f)
			return new ForbiddenProblemResult($"Not enough Stock! (Current: {stock.Quantity}, Delta: {delta})");

		stock.Quantity += delta;
		return await PutStock(stock);
	}

	public async Task<IActionResult> DeleteStock(int id) {
		ActionResult<Stock> stockRes = await GetStock(id);
		if (stockRes.Result != null) return stockRes.Result;

		_context.Stock.Remove(stockRes.Value!);
		await _context.SaveChangesAsync();

		return new OkResult();
	}

	public async Task<bool> StockExists(int id) => 
		await _context.Stock.AnyAsync(e => e.StockID == id);
}
