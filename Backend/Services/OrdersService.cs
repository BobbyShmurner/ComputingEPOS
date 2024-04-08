using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using ComputingEPOS.Backend.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace ComputingEPOS.Backend.Services;

public class OrdersService : IOrdersService {
    private readonly BaseDbContext _context;

    public OrdersService(BaseDbContext context) =>  _context = context;

    public async Task<ActionResult<List<Order>>> GetOrders(bool? closed, int? parentId, DateTime? from, DateTime? to) =>
        await _context.Orders.Where(x
            => (closed == null || x.IsClosed == closed)
            && (parentId == null || x.ParentOrderID == parentId)
            && (from == null || x.Date >= from)
            && (to == null || x.Date <= to)
        ).ToListAsync();

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

    public async Task<ActionResult<Order?>> GetParentOrder(int id) {
		ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        if (order.ParentOrderID == null) return new ActionResult<Order?>(value: null);

        ActionResult<Order> parentOrderResult = await GetOrder(order.ParentOrderID.Value);
        if (parentOrderResult.Result != null) return parentOrderResult.Result!;
        return parentOrderResult.Value;
    }

    public async Task<ActionResult<List<Order>>> GetChildOrders(int id, DateTime? from, DateTime? to) {
		ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        return await GetOrders(closed: null, parentId: order.OrderID, from: from, to: to);
    }

    public async Task<ActionResult<List<Order>>> GetAllRelatedOrders(int id) {
        Order parentOrder;

        if (await IsChildOrder(id)) {
            ActionResult<Order?> parentOrderRes = await GetParentOrder(id);
            if (parentOrderRes.Result != null) return parentOrderRes.Result;
            if (parentOrderRes.Value == null)
                return new ProblemResult($"Order {id} has no parent, despite IsChildOrder({id}) returning true.");

            parentOrder = parentOrderRes.Value;
        } else {
            ActionResult<Order> parentOrderRes = await GetOrder(id);
            if (parentOrderRes.Result != null) return parentOrderRes.Result;
            parentOrder = parentOrderRes.Value!;
        }

        ActionResult<List<Order>> childOrdersRes = await GetChildOrders(parentOrder.OrderID, from: null, to: null);
        if (childOrdersRes.Result != null) return childOrdersRes.Result;
        
        childOrdersRes.Value!.Add(parentOrder);
        return childOrdersRes.Value!;
    }

    public async Task<ActionResult<List<OrderItem>>> GetOrderItems(int id, int? stockId, IOrderItemsService orderItemsService)
        => await orderItemsService.GetOrderItems(id, stockId);

    public async Task<ActionResult<List<Transaction>>> GetOrderTransactions(int id, ITransactionsService transactionsService)
        => await transactionsService.GetTransactions(id);

    public async Task<ActionResult<decimal>> GetOrderCost(int id, IOrderItemsService orderItemsService) {
        ActionResult<List<OrderItem>> itemsResult = await GetOrderItems(id, null, orderItemsService);
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

    public async Task<ActionResult<Order>> PostOrder(Order order, int? parentId) {
        if (order.OrderNum == null)
        {
            ActionResult<int> orderNumResult = await GetNextOrderNum();
            if (orderNumResult.Result != null) return orderNumResult.Result;
            order.OrderNum = orderNumResult.Value!;
        }

        if (parentId != null) order.ParentOrderID = parentId;
        if (order.ParentOrderID != null) {
            if (!await OrderExists(order.ParentOrderID.Value)) return new NotFoundResult();
            if (await IsChildOrder(order.ParentOrderID.Value))
                return new ForbiddenProblemResult($"Order {order.ParentOrderID.Value} is a child order, and cannot be a parent order. Try using it's parent instead.");
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<ActionResult<Order>> FinaliseOrder(int id) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result;
        Order order = orderResult.Value!;

        order.OrderDuration = (DateTime.Now - order.Date).TotalSeconds;
        return await PutOrder(order);
    }

    public async Task<ActionResult<Order>> CompleteOrder(int id) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        order.PrepDuration = (DateTime.Now - order.Date).TotalSeconds - order.OrderDuration;
        return await PutOrder(order);
    }

    public async Task<ActionResult<Order>> ForceCloseCheck(int id)
        => await CloseCheck_Internal(id, true, null, null);

    public async Task<ActionResult<Order>> CloseCheck(int id, ITransactionsService transactionsService, IOrderItemsService? orderItemsService)
        => await CloseCheck_Internal(id, false, transactionsService, orderItemsService);

    public async Task<IActionResult> CloseAllPaidChecks(bool closeEmpty, ITransactionsService transactionsService, IOrderItemsService orderItemsService) {
        ActionResult<List<Order>> ordersResult = await GetOrders(closed: false, parentId: null, from: null, to: null);
        if (ordersResult.Result != null) return ordersResult.Result!;
        List<Order> orders = ordersResult.Value!;

        foreach (var order in orders) {
            var amountDueRes = await GetAmountDue(order.OrderID, transactionsService, orderItemsService);
            if (amountDueRes.Result != null) return amountDueRes.Result!;
            var amountDue = amountDueRes.Value!;

            if (amountDue != 0) continue;
            if (!closeEmpty) {
                var orderItemsRes = await GetOrderItems(order.OrderID, null, orderItemsService);
                if (orderItemsRes.Result != null) return orderItemsRes.Result!;
                var orderItems = orderItemsRes.Value!;

                if (orderItems.Count == 0) continue;
            }

            // Force close, as we already checked if it is able to close
            await CloseCheck_Internal(order, true, transactionsService, orderItemsService);
        }

        return new OkResult();
    }

    async Task<ActionResult<Order>> CloseCheck_Internal(int id, bool forced, ITransactionsService? transactionsService, IOrderItemsService? orderItemsService) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        return await CloseCheck_Internal(order: order, forced: forced, transactionsService: transactionsService, orderItemsService: orderItemsService);
    }

    async Task<ActionResult<Order>> CloseCheck_Internal(Order order, bool forced, ITransactionsService? transactionsService, IOrderItemsService? orderItemsService) {
        if (!forced) {
            ActionResult<decimal> amountDueResult = await GetAmountDue(order.OrderID, transactionsService!, orderItemsService!);
            if (amountDueResult.Result != null) return amountDueResult.Result!;
            decimal amountDue = amountDueResult.Value!;

            if (amountDue > 0)
                return new ForbiddenProblemResult($"Order must be fully paid for before closing. ({amountDue} due)");
            if (amountDue < 0)
                return new ForbiddenProblemResult($"Change must be given before closing. ({-amountDue} change due)");
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

    public async Task<IActionResult> DeleteOrder(int id, IOrderItemsService orderItemsService, IStockService stockService, ITransactionsService transactionsService) {
        ActionResult<Order> orderResult = await GetOrder(id);
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        ActionResult<List<Transaction>> transactionsRes = await GetOrderTransactions(id, transactionsService);
        if (transactionsRes.Result != null) return transactionsRes.Result!;
        List<Transaction> transactions = transactionsRes.Value!;

        if (transactions.Count > 0) {
            return new ForbiddenProblemResult($"Cannot delete an order that has transactions associated with it");
        }

        ActionResult<List<OrderItem>> orderItemsRes = await GetOrderItems(id, stockId: null, orderItemsService);
        if (orderItemsRes.Result != null) return orderItemsRes.Result!;
        List<OrderItem> orderItems = orderItemsRes.Value!;

        foreach (var item in orderItems) {
            IActionResult res = await orderItemsService.DeleteOrderItem(item.OrderItemID, this, stockService);
            if (res is not OkResult && res is not OkObjectResult) return res;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<bool> OrderExists(int id) => 
        await _context.Orders.AnyAsync(e => e.OrderID == id);

    public async Task<bool> IsChildOrder(int id) =>
        await _context.Orders.AnyAsync(e => e.OrderID == id && e.ParentOrderID != null);

    public async Task<ActionResult<decimal>> GetAmountPaid(int id, ITransactionsService transactionsService) {
        decimal amountPaid = 0;

        ActionResult<List<Transaction>> transactionsResult = await GetOrderTransactions(id, transactionsService);
        if (transactionsResult.Result != null) return transactionsResult.Result!;
        transactionsResult.Value!.ForEach(x => amountPaid += x.AmountPaid);

        return amountPaid;
    }

    public async Task<ActionResult<decimal>> GetAmountDue(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService) {
        ActionResult<decimal> orderCostResult = await GetOrderCost(id, orderItemsService);
        if (orderCostResult.Result != null) return orderCostResult.Result!;
        decimal orderCost = orderCostResult.Value!;

        ActionResult<decimal> amountPaidResult = await GetAmountPaid(id, transactionsService);
        if (amountPaidResult.Result != null) return amountPaidResult.Result!;
        decimal amountPaid = amountPaidResult.Value!;

        return orderCost - amountPaid;
    }

    public async Task<ActionResult<Order?>> GetLatestOrder() {
        try {
            Order? order = await _context.Orders.OrderBy(order => order.Date).LastOrDefaultAsync();
            return new ActionResult<Order?>(value: order);
        } catch {
            return new StatusCodeResult(500);
        }
    }

    public async Task<ActionResult<int>> GetNextOrderNum() {
        ActionResult<Order?> orderResult = await GetLatestOrder();
        if (orderResult.Result != null) return orderResult.Result!;
        Order order = orderResult.Value!;

        if (order.Date.Date == DateTime.Today) {
            if (order.OrderNum.HasValue && order.OrderNum.Value != 99)
                return order.OrderNum.Value + 1;
            else
                return 1;
        } else {
            return 1;
        }
    }
}
