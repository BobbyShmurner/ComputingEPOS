using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;
using ComputingEPOS.Backend.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ComputingEPOS.Backend.Services;

public class OrdersService : IOrdersService {
    private readonly BaseDbContext _context;

    public OrdersService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<Order>>> GetOrders(bool? closed) {
		if (closed == null) return await _context.Orders.ToListAsync();
        return await _context.Orders.Where(x => x.IsClosed == closed).ToListAsync();
    }

    public async Task<ActionResult<Order>> GetOrder(int id) {
		Order? order = await _context.Orders.FindAsync(id);
        return order != null ? order : new NotFoundResult();
    }

    public async Task<ActionResult<Order>> GetOrderFromOrderNum([BindRequired]int orderNum, bool todayOnly) {
		Order? order = await _context.Orders
            .OrderBy(x => x.Date)
            .LastOrDefaultAsync(x => x.OrderNum == orderNum && (!todayOnly || x.Date.Date == DateTime.Today));

        return order != null ? order : new NotFoundResult();
    }

    public async Task<ActionResult<List<OrderItem>>> GetOrderItems(int id, IOrderItemsService orderItemsService)
        => await orderItemsService.GetOrderItems(id);

    public async Task<ActionResult<List<Transaction>>> GetOrderTransactions(int id, ITransactionsService transactionsService)
        => await transactionsService.GetTransactions(id);

    public async Task<ActionResult<decimal>> GetOrderCost(int id, IOrderItemsService orderItemsService) {
        ActionResult<List<OrderItem>> itemsResult = await GetOrderItems(id, orderItemsService);
        if (itemsResult.Result != null) return itemsResult.Result!;

        List<OrderItem> items = itemsResult.Value!;
        decimal cost = 0;
        
        items.ForEach(x => cost += x.Subtotal);
        return cost;
    }

    public async Task<ActionResult<Order>> PutOrder(Order order) {
        _context.Entry(order).State = EntityState.Modified;

        try {
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException) {
			if (!await OrderExists(order.OrderID)) return new NotFoundResult();
			else throw;
		}

        return order;
    }

    public async Task<ActionResult<Order>> PostOrder(Order order) {
        _context.Orders.Add(order);

        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<ActionResult<Order>> FinaliseOrder(int id) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        order.OrderDuration = (DateTime.Now - order.Date).TotalSeconds;
        return await PutOrder(order);
    }

    public async Task<ActionResult<Order>> ForceCloseCheck(int id)
        => await CloseCheck_Internal(id, true, null, null);

    public async Task<ActionResult<Order>> CloseCheck(int id, ITransactionsService transactionsService, IOrderItemsService? orderItemsService)
        => await CloseCheck_Internal(id, false, transactionsService, orderItemsService);

    async Task<ActionResult<Order>> CloseCheck_Internal(int id, bool forced, ITransactionsService? transactionsService, IOrderItemsService? orderItemsService) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        if (!forced) {
            ActionResult<decimal> amountDueResult = await GetAmountDueForOrder(order.OrderID, transactionsService!, orderItemsService!);
            if (amountDueResult.Result != null) return amountDueResult.Result!;
            decimal amountDue = amountDueResult.Value!;

            if (amountDue > 0)
                return new ProblemResult($"Order must be fully paid for before closing. ({amountDue} due)", statusCode: 403);
            if (amountDue < 0)
                return new ProblemResult($"Change must be given before closing. ({-amountDue} change due)", statusCode: 403);
        }

        order.IsClosed = true;
        return await PutOrder(order);
    }

    public async Task<IActionResult> ForceCloseAllChecks() {
        bool success = true;
        await _context.Orders.ForEachAsync(async x => {
            if ((await ForceCloseCheck(x.OrderID)).Result != null) success = false;
        });
        
        return success ? new OkResult() : new StatusCodeResult(500);
    }

    public async Task<IActionResult> DeleteOrder(int id) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> OrderExists(int id) => 
        await _context.Orders.AnyAsync(e => e.OrderID == id);

    public async Task<ActionResult<decimal>> GetAmountDueForOrder(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        ActionResult<decimal> amountDueResult = await GetOrderCost(id, orderItemsService);
        if (amountDueResult.Result != null) return amountDueResult.Result!;
        decimal amountDue = amountDueResult.Value!;

        ActionResult<List<Transaction>> transactionsResult = await GetOrderTransactions(id, transactionsService);
        if (transactionsResult.Result != null) return transactionsResult.Result!;
        transactionsResult.Value!.ForEach(x => amountDue -= x.AmountPaid);

        return amountDue;
    }
}
