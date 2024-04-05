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

	public async Task<ActionResult<PmixReport>> GetStockPmix(int id, DateTime? from, DateTime? to, IOrdersService ordersService, IOrderItemsService orderItemsService) {
		ActionResult<Stock> stockRes = await GetStock(id);
        if (stockRes.Result != null) return stockRes.Result;
        Stock stock = stockRes.Value!;

		ActionResult<List<PmixReport>> reportsRes = await GetStockPmix_Internal(new List<Stock> { stock }, from, to, ordersService, orderItemsService, includeRemainingAsUnknown: false);
        if (reportsRes.Result != null) return reportsRes.Result;

        return reportsRes.Value!.First();
    }

    public async Task<ActionResult<List<PmixReport>>> GetAllStockPmix(DateTime? from, DateTime? to, IOrdersService ordersService, IOrderItemsService orderItemsService)
    {
        ActionResult<List<Stock>> allStockRes = await GetAllStock();
        if (allStockRes.Result != null) return allStockRes.Result;
        List<Stock> allStock = allStockRes.Value!;

		return await GetStockPmix_Internal(allStock, from, to, ordersService, orderItemsService, includeRemainingAsUnknown: true);
    }

    async Task<ActionResult<List<PmixReport>>> GetStockPmix_Internal(List<Stock> stocks, DateTime? from, DateTime? to, IOrdersService ordersService, IOrderItemsService orderItemsService, bool includeRemainingAsUnknown) {
		from ??= DateTime.MinValue;
		to ??= DateTime.Now;

        ActionResult<List<Order>> ordersRes = await ordersService.GetOrders(closed: true, parentId: null, from: from, to: to);
        if (ordersRes.Result != null) return ordersRes.Result;
        List<Order> orders = ordersRes.Value!;

        List<OrderItem> orderItems = new();
		List<PmixReport> reports = new();

        foreach (var order in orders) {
			ActionResult<List<OrderItem>> orderItemsRes = await orderItemsService.GetOrderItems(order.OrderID, stockId: null);
			if (orderItemsRes.Result != null) return orderItemsRes.Result;
			orderItems.AddRange(orderItemsRes.Value!);
		}

		foreach (var stock in stocks) {
            float quantitySold = 0f;
            decimal gross = 0m;

			var items = orderItems.Where(item => item.StockID == stock.StockID);
            foreach (var item in items) {
                quantitySold += item.Quantity;
                gross += item.Subtotal;
            }

			orderItems = orderItems.Except(items).ToList();

            reports.Add(new PmixReport {
                Stock = stock,
                From = from.Value,
                To = to.Value,
                QuantitySold = quantitySold,
                Gross = gross,
            });
        }

		if (includeRemainingAsUnknown && orderItems.Count > 0) {
			float quantitySold = 0f;
			decimal gross = 0m;

			foreach (var item in orderItems) {
				quantitySold += item.Quantity;
				gross += item.Subtotal;

				Console.WriteLine($"Unknown Stock (ID: {item.StockID}) (Quantity: {item.Quantity}) (Subtotal: {item.Subtotal}), (OrderItemID: {item.OrderItemID}), (OrderID: {item.OrderID})");
			}

			reports.Add(new PmixReport {
				Stock = new Stock { Name = "[UNKNOWN]", StockID = -1, Quantity = 0 },
				From = from.Value,
				To = to.Value,
				QuantitySold = quantitySold,
				Gross = gross,
			});
		}

		return reports;
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
