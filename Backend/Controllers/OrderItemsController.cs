using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemsController : ControllerBase {
    private readonly IOrderItemsService m_Service;
	private readonly IOrdersService m_OrdersService;

	public OrderItemsController(IOrderItemsService service, IOrdersService ordersService) {
		 m_Service = service;
		 m_OrdersService = ordersService;
	}

	// GET: api/OrderItems
	[HttpGet]
	public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int? orderId = null)
		=> await m_Service.GetOrderItems(orderId);

	// GET: api/OrderItems/5
	[HttpGet("{id}")]
	public async Task<ActionResult<OrderItem>> GetOrderItem(int id) {
		OrderItem? orderItem = await m_Service.GetOrderItem(id);
		return orderItem != null ? orderItem : NotFound();
	}

	// PUT: api/OrderItems/5
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderItem>> PutOrderItem(int id, OrderItem orderItem) {
        if (id != orderItem.OrderItemID) return BadRequest(); 
        
        OrderItem? updatedOrderItem = await m_Service.PutOrderItem(orderItem);
        return updatedOrderItem != null ? updatedOrderItem : NotFound();
    }

    // POST: api/OrderItems
    [HttpPost]
    public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)  {
        OrderItem? newOrderItem = await m_Service.PostOrderItem(orderItem);
        return CreatedAtAction(nameof(GetOrderItem), new { id = newOrderItem.OrderItemID }, newOrderItem);
    }

    // DELETE: api/OrderItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id) {
        if (!await m_Service.DeleteOrderItem(id)) return NotFound();
        return Ok();
    }
}
