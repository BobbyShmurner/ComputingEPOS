using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public interface IOrdersService {
    public Task<ActionResult<List<Order>>> GetOrders(bool? closed);
    public Task<ActionResult<Order>> GetOrder(int id);
    public Task<ActionResult<Order>> GetOrderFromOrderNum(int orderNum, bool todayOnly);
    public Task<ActionResult<decimal>> GetOrderCost(int id, IOrderItemsService orderItemsService);
    public Task<ActionResult<List<OrderItem>>> GetOrderItems(int id, IOrderItemsService orderItemsService);
    public Task<ActionResult<List<Transaction>>> GetOrderTransactions(int id, ITransactionsService transactionsService);
    public Task<ActionResult<decimal>> GetAmountDueForOrder(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService);

    public Task<ActionResult<Order>> PutOrder(Order order);

    public Task<ActionResult<Order>> PostOrder(Order order);
    public Task<ActionResult<Order>> FinaliseOrder(int id);
    public Task<ActionResult<Order>> CloseCheck(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService);
    public Task<ActionResult<Order>> ForceCloseCheck(int id);
    public Task<IActionResult> ForceCloseAllChecks();

    public Task<IActionResult> DeleteOrder(int id);

    public Task<bool> OrderExists(int id);
}
