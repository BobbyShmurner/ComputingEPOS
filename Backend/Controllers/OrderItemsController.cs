using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemsController : ControllerBase {
    private readonly IOrderItemsService m_Service;
	private readonly IOrdersService m_OrdersService;
	private readonly IStockService m_StockService;

	public OrderItemsController(IOrderItemsService service, IOrdersService ordersService, IStockService stockService) {
		 m_Service = service;
		 m_StockService = stockService;
		 m_OrdersService = ordersService;
	}

	// GET: api/OrderItems
	[HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<OrderItem>>> GetOrderItems(int? orderId = null) =>
		await m_Service.GetOrderItems(orderId);

	// GET: api/OrderItems/5
	[HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<OrderItem>> GetOrderItem(int id) =>
		await m_Service.GetOrderItem(id);

    // GET: api/OrderItems/5/Order
	[HttpGet("{id}/Order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Order>> GetOrder(int id) =>
		await m_Service.GetOrder(id, m_OrdersService);

    // GET: api/OrderItems/5/Stock
	[HttpGet("{id}/Stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Stock>> GetStock(int id) =>
		await m_Service.GetStock(id, m_StockService);

	// PUT: api/OrderItems/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItem>> PutOrderItem(int id, OrderItem orderItem) {
        if (orderItem.OrderItemID != id) return BadRequest();
        return await m_Service.PutOrderItem(orderItem, m_StockService);
    }

    // // POST: api/OrderItems
    // [HttpPost]
    // [ProducesResponseType(StatusCodes.Status201Created)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem) {
    //     ActionResult<OrderItem> newOrderItemRes = await m_Service.PostOrderItem(orderItem, m_StockService);
    //     if (newOrderItemRes.Result != null) return newOrderItemRes.Result;
    //     OrderItem newOrderItem = newOrderItemRes.Value!;

    //     return CreatedAtAction(nameof(GetOrderItems), new { id = newOrderItem.OrderItemID }, newOrderItem);
    // }

    // POST: api/OrderItems
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<OrderItem>>> PostOrderItems(ICollection<OrderItem> orderItems) {
        List<OrderItem> newOrderItems = new();

        foreach (OrderItem orderItem in orderItems) {
            ActionResult<OrderItem> newOrderItemRes = await m_Service.PostOrderItem(orderItem, m_StockService);
            if (newOrderItemRes.Result != null) return newOrderItemRes.Result;
            
            newOrderItems.Add(newOrderItemRes.Value!);
        }

        return CreatedAtAction(nameof(GetOrderItems), newOrderItems);
    }

    // DELETE: api/OrderItems/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrderItem(int id) =>
        await m_Service.DeleteOrderItem(id);
}
