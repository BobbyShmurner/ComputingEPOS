using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Models;

namespace ComputingEPOS.Backend.Services;

public interface IOrdersService {
    public Task<ActionResult<List<Order>>> GetOrders(bool? closed, int? parentId);
    public Task<ActionResult<Order>> GetOrder(int id);
    public Task<ActionResult<Order?>> GetLatestOrder();
    public Task<ActionResult<int>> GetNextOrderNum();
    public Task<ActionResult<Order>> GetOrderFromOrderNum(int orderNum, bool todayOnly);
    public Task<ActionResult<Order?>> GetParentOrder(int id);
    public Task<ActionResult<List<Order>>> GetChildOrders(int id);
    public Task<ActionResult<List<Order>>> GetAllRelatedOrders(int id);
    public Task<ActionResult<decimal>> GetOrderCost(int id, IOrderItemsService orderItemsService);
    public Task<ActionResult<List<OrderItem>>> GetOrderItems(int id, IOrderItemsService orderItemsService);
    public Task<ActionResult<List<Transaction>>> GetOrderTransactions(int id, ITransactionsService transactionsService);
    public Task<ActionResult<decimal>> GetAmountDueForOrder(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService);

    public Task<ActionResult<Order>> PutOrder(Order order);

    public Task<ActionResult<Order>> PostOrder(Order order, int? parentId);
    public Task<ActionResult<Order>> FinaliseOrder(int id);
    public Task<ActionResult<Order>> CloseCheck(int id, ITransactionsService transactionsService, IOrderItemsService orderItemsService);
    public Task<ActionResult<Order>> ForceCloseCheck(int id);
    public Task<ActionResult<Order>> CompleteOrder(int id);
    public Task<IActionResult> ForceCloseAllChecks();

    public Task<IActionResult> DeleteOrder(int id);

    public Task<bool> OrderExists(int id);
    public Task<bool> IsChildOrder(int id);
}
