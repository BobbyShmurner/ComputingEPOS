using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public class OrderItemsService : IOrderItemsService {
    private readonly BaseDbContext _context;

    public OrderItemsService(BaseDbContext context) =>  _context = context;

    public async Task<List<OrderItem>> GetOrderItems(int? orderId) {
        if (orderId == null) return await _context.OrderItems.ToListAsync();
		return await _context.OrderItems.Where(x => x.OrderID == orderId).ToListAsync();
    }

    public async Task<OrderItem?> GetOrderItem(int id)
		=> await _context.OrderItems.FindAsync(id);

    public async Task<Order?> GetOrder(int id, IOrdersService ordersService) {
        OrderItem? item = await GetOrderItem(id);
        if (item == null) return null;

		return await ordersService.GetOrder(item.OrderID);
    }

    public async Task<OrderItem?> PutOrderItem(OrderItem orderItem) {
        _context.Entry(orderItem).State = EntityState.Modified;

        try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await OrderItemExists(orderItem.OrderItemID)) return null;
			else throw;
		}

        return orderItem;
    }

    public async Task<OrderItem> PostOrderItem(OrderItem orderItem)  {
        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();
        return orderItem;
    }

    public async Task<bool> DeleteOrderItem(int id) {
        OrderItem? orderItem = await _context.OrderItems.FindAsync(id);
        if (orderItem == null) return false;

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> OrderItemExists(int id) => 
        await _context.OrderItems.AnyAsync(e => e.OrderItemID == id);
}
