using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public interface IOrderItemsService {
    public Task<List<OrderItem>> GetOrderItems(int? orderId);
    public Task<OrderItem?> GetOrderItem(int id);
    public Task<Order?> GetOrder(int id, IOrdersService ordersService);

    public Task<OrderItem?> PutOrderItem(OrderItem orderItem);

    public Task<OrderItem> PostOrderItem(OrderItem orderItem);

    public Task<bool> DeleteOrderItem(int id);

    public Task<bool> OrderItemExists(int id);
}
