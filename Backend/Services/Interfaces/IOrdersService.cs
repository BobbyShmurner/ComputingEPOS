using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public interface IOrdersService {
    public Task<List<Order>> GetOrders();
    public Task<Order?> GetOrder(int id);
    public Task<int?> GetOrderIdFromOrderNum(int orderNum, bool todayOnly = true);
    public Task<List<OrderItem>> GetOrderItems(int id, IOrderItemsService orderItemsService);

    public Task<Order?> PutOrder(Order order);

    public Task<Order> PostOrder(Order order);
    public Task<Order?> CloseCheck(int id, bool forced);
    public Task<bool> ForceCloseAllChecks();

    public Task<bool> DeleteOrder(int id);

    public Task<bool> OrderExists(int id);
}
