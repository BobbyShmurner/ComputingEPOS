using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using ComputingEPOS.Backend.Results;
using System.Text.Json;

namespace ComputingEPOS.Backend.Services;

public class OrderItemsService : IOrderItemsService {
    private readonly BaseDbContext m_Context;

    public OrderItemsService(BaseDbContext context) =>  m_Context = context;

    public async Task<ActionResult<List<OrderItem>>> GetOrderItems(int? orderId) {
        if (orderId == null) return await m_Context.OrderItems.ToListAsync();
		return await m_Context.OrderItems.Where(x => x.OrderID == orderId).ToListAsync();
    }

    public async Task<ActionResult<OrderItem>> GetOrderItem(int id) {
		OrderItem? orderItem = await m_Context.OrderItems.FindAsync(id);
        return orderItem != null ? orderItem : new NotFoundResult();
    }

    public async Task<ActionResult<Order>> GetOrder(int id, IOrdersService ordersService) {
        ActionResult<OrderItem> itemResult = await GetOrderItem(id);
        if (itemResult.Result != null) return itemResult.Result!;

		return await ordersService.GetOrder(itemResult.Value!.OrderID);
    }

    public async Task<ActionResult<Stock>> GetStock(int id, IStockService stockService) {
        ActionResult<OrderItem> itemResult = await GetOrderItem(id);
        if (itemResult.Result != null) return itemResult.Result!;

		return await stockService.GetStock(itemResult.Value!.StockID);
    }

    public async Task<ActionResult<OrderItem>> PutOrderItem(OrderItem orderItem, IStockService stockService) {
        using var transaction = m_Context.Database.BeginTransaction();

        ActionResult<OrderItem> originalItemRes = await GetOrderItem(orderItem.OrderItemID);
        if (originalItemRes.Result != null) return originalItemRes.Result;
        OrderItem originalItem = originalItemRes.Value!;

        ActionResult<Stock> stockRes = await stockService.GetStock(orderItem.StockID);
        if (stockRes.Result != null) return stockRes.Result;
        Stock stock = stockRes.Value!;

        float delta = orderItem.Quantity - originalItem.Quantity;
        ActionResult<Stock> stockUpdateRes = await stockService.UpdateStockQuantity(orderItem.StockID, -delta);
        if (stockUpdateRes.Result != null) return stockUpdateRes.Result;

        m_Context.Entry(orderItem).State = EntityState.Modified;

        try {
			await m_Context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await OrderItemExists(orderItem.OrderItemID)) return new NotFoundResult();
			else throw;
		}

        await transaction.CommitAsync();
        return orderItem;
    }

    public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem, IStockService stockService) {
        using var transaction = m_Context.Database.BeginTransaction();

        ActionResult<Stock> stockRes = await stockService.GetStock(orderItem.StockID);
        if (stockRes.Result != null) return stockRes.Result;
        Stock stock = stockRes.Value!;

        ActionResult<Stock> stockUpdateRes = await stockService.UpdateStockQuantity(orderItem.StockID, -orderItem.Quantity);
        if (stockUpdateRes.Result != null) return stockUpdateRes.Result;

        m_Context.OrderItems.Add(orderItem);
        await m_Context.SaveChangesAsync();
        await transaction.CommitAsync();

        return orderItem;
    }

    public async Task<IActionResult> DeleteOrderItem(int id, IStockService stockService) {
        using var transaction = m_Context.Database.BeginTransaction();

        ActionResult<OrderItem> itemResult = await GetOrderItem(id);
        if (itemResult.Result != null) return itemResult.Result;
        OrderItem orderItem = itemResult.Value!;

        ActionResult<Stock> stockRes = await stockService.GetStock(orderItem.StockID);
        if (stockRes.Result != null) return stockRes.Result;
        Stock stock = stockRes.Value!;

        ActionResult<Stock> stockUpdateRes = await stockService.UpdateStockQuantity(orderItem.StockID, orderItem.Quantity);
        if (stockUpdateRes.Result != null) return stockUpdateRes.Result;

        Console.WriteLine($"Removing OrderItem ID: {orderItem.OrderItemID}, passed ID: {id}");

        m_Context.OrderItems.Remove(orderItem);
        await m_Context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkResult();
    }

    public async Task<bool> OrderItemExists(int id) => 
        await m_Context.OrderItems.AnyAsync(e => e.OrderItemID == id);
}
