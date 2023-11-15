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

public class OrdersService : IOrdersService {
    private readonly BaseDbContext _context;

    public OrdersService(BaseDbContext context) =>  _context = context;

    public async Task<List<Order>> GetOrders() => 
		await _context.Orders.ToListAsync();

    public async Task<Order?> GetOrder(int id) 
		=> await _context.Orders.FindAsync(id);

    public async Task<int?> GetOrderIdFromOrderNum([BindRequired]int orderNum, bool todayOnly = true)
		=> (await _context.Orders
            .OrderBy(x => x.Date)
            .LastOrDefaultAsync(x => x.OrderNum == orderNum && (!todayOnly || x.Date.Date == DateTime.Today)))
            ?.OrderID;

    public async Task<List<OrderItem>> GetOrderItems(int id, IOrderItemsService orderItemsService)
        => await orderItemsService.GetOrderItems(orderId:id);

    public async Task<Order?> PutOrder(Order order) {
        _context.Entry(order).State = EntityState.Modified;

        try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await OrderExists(order.OrderID)) return null;
			else throw;
		}

        return order;
    }

    public async Task<Order> PostOrder(Order order)  {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> CloseCheck(int id, bool forced) {
        Order? order = await GetOrder(id);
        if (order == null) return null;

        // TODO: Add check to ensure order is paid for

        order.IsClosed = true;
        return await PutOrder(order);
    }

    public async Task<bool> ForceCloseAllChecks() {
        bool success = true;
        await _context.Orders.ForEachAsync(async x => {
            if (await CloseCheck(x.OrderID, true) == null) success = false;
        });
        
        return success;
    }

    public async Task<bool> DeleteOrder(int id) {
        Order? order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> OrderExists(int id) => 
        await _context.Orders.AnyAsync(e => e.OrderID == id);
}
