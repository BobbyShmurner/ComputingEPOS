using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Backend.Models;
using ComputingEPOS.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase {
    private readonly IOrdersService m_Service;
    private readonly IOrderItemsService m_OrderItemsService;

    public OrdersController(IOrdersService service, IOrderItemsService orderItemsService) {
         m_Service = service;
         m_OrderItemsService = orderItemsService;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        => await m_Service.GetOrders();

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id) {
        Order? order = await m_Service.GetOrder(id);
        return order != null ? order : NotFound();
    }

    // GET: api/Orders/FromOrderNum
    [HttpGet("FromOrderNum")]
    public async Task<ActionResult<Order>> GetOrderFromOrderNum([Required]int orderNum, bool todayOnly = true) {
       int? orderId = await m_Service.GetOrderIdFromOrderNum(orderNum, todayOnly);
       if (orderId == null) return NotFound();

       return await GetOrder(orderId.Value);
    }

    // GET: api/Orders/5/OrderItems
    [HttpGet("{id}/OrderItems")]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        => await m_Service.GetOrderItems(id, m_OrderItemsService);

    // PUT: api/Orders/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Order>> PutOrder(int id, Order order) {
        if (id != order.OrderID) return BadRequest(); 
        
        Order? updatedOrder = await m_Service.PutOrder(order);
        return updatedOrder != null ? updatedOrder : NotFound();
    }

    // POST: api/Orders/5/CloseCheck
    [HttpPost("{id}/CloseCheck")]
    public async Task<ActionResult<Order>> PostCloseCheck(int id) {
        Order? updatedOrder = await m_Service.CloseCheck(id, false);
        return updatedOrder != null ? updatedOrder : NotFound();
    }

    // POST: api/Orders/5/ForceCloseCheck
    [HttpPost("{id}/ForceCloseCheck")]
    public async Task<ActionResult<Order>> PostForceCloseCheck(int id) {
        Order? updatedOrder = await m_Service.CloseCheck(id, true);
        return updatedOrder != null ? updatedOrder : NotFound();
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(Order order)  {
        Order? newOrder = await m_Service.PostOrder(order);
        return CreatedAtAction(nameof(GetOrder), new { id = newOrder.OrderID }, newOrder);
    }

    // POST: api/Orders/ForceCloseAllChecks
    [HttpPost("ForceCloseAllChecks")]
    public async Task<IActionResult> PostForceCloseAllChecks() =>
        await m_Service.ForceCloseAllChecks() ? Ok() : Problem("Failed to force close all open checks.");

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id) {
        if (!await m_Service.DeleteOrder(id)) return NotFound();
        return Ok();
    }
}
