using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public interface IOrderItemsService {
    public Task<ActionResult<List<OrderItem>>> GetOrderItems(int? orderId, int? stockId);
    public Task<ActionResult<OrderItem>> GetOrderItem(int id);
    public Task<ActionResult<Order>> GetOrder(int id, IOrdersService ordersService);
    public Task<ActionResult<Stock>> GetStock(int id, IStockService stockService);

    // public Task<ActionResult<OrderItem>> PutOrderItem(OrderItem orderItem, IStockService stockService);

    public Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem, IOrdersService ordersService, IStockService stockService);

    public Task<IActionResult> DeleteDanglingOrderItems(IOrdersService ordersService, IStockService stockService);
    public Task<IActionResult> DeleteOrderItem(int id, IOrdersService ordersService, IStockService stockService);

    public Task<bool> OrderItemExists(int id);
}
