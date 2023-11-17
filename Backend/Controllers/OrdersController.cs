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
    private readonly ITransactionsService m_TransactionsService;

    public OrdersController(IOrdersService service, IOrderItemsService orderItemsService, ITransactionsService transactionsService) {
         m_Service = service;
         m_OrderItemsService = orderItemsService;
         m_TransactionsService = transactionsService;
    }

    // GET: api/Orders?closed=false&parentId=5
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Order>>> GetOrders(bool? closed = null, int? parentId = null) =>
        await m_Service.GetOrders(closed, parentId);

    // GET: api/Orders/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetOrder(int id) =>
        await m_Service.GetOrder(id);

    // GET: api/Orders/5/Parent
    [HttpGet("{id}/Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order?>> GetParentOrder(int id) =>
        await m_Service.GetParentOrder(id);

    // GET: api/Orders/5/Parent
    [HttpGet("{id}/Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Order>>> GetChildOrders(int id) =>
        await m_Service.GetChildOrders(id);

    // GET: api/Orders/5/AllRelated
    [HttpGet("{id}/AllRelated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Order>>> GetAllRelatedOrders(int id) =>
        await m_Service.GetAllRelatedOrders(id);

    // GET: api/Orders/5/Cost
    [HttpGet("{id}/Cost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<decimal>> GetOrderCost(int id) =>
        await m_Service.GetOrderCost(id, m_OrderItemsService);

    // GET: api/Orders/5/AmountDue
    [HttpGet("{id}/AmountDue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<decimal>> GetAmountDueForOrder(int id) =>
        await m_Service.GetAmountDueForOrder(id, m_TransactionsService, m_OrderItemsService);

    // GET: api/Orders/FromOrderNum
    [HttpGet("FromOrderNum")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetOrderFromOrderNum([Required]int orderNum, bool todayOnly = true) =>
       await m_Service.GetOrderFromOrderNum(orderNum, todayOnly);

    // GET: api/Orders/5/OrderItems
    [HttpGet("{id}/OrderItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderItem>>> GetOrderItems(int id) =>
        await m_Service.GetOrderItems(id, m_OrderItemsService);

    // GET: api/Orders/5/Transactions
    [HttpGet("{id}/Transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Transaction>>> GetOrderTransactions(int id) =>
        await m_Service.GetOrderTransactions(id, m_TransactionsService);

    // PUT: api/Orders/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> PutOrder(int id, Order order) {
        if (id != order.OrderID) return BadRequest(); 
        return await m_Service.PutOrder(order);
    }

    // POST: api/Orders/5/CloseCheck?force=false
    [HttpPost("{id}/CloseCheck")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Order>> PostCloseCheck(int id, bool force = false) =>
        force
            ? await m_Service.ForceCloseCheck(id)
            : await m_Service.CloseCheck(id, m_TransactionsService, m_OrderItemsService);

    // POST: api/Orders/5/Finalise
    [HttpPost("{id}/Finalise")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> PostFinaliseOrder(int id) =>
        await m_Service.FinaliseOrder(id);

    // POST: api/Orders/5/Complete
    [HttpPost("{id}/Complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> PostCompleteOrder(int id) =>
        await m_Service.CompleteOrder(id);

    // POST: api/Orders?parentId=5
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> PostOrder(Order order, int? parentId = null) {
        if (order.ParentOrderID != null && parentId != null && order.ParentOrderID != parentId)
            return BadRequest("Missmatched Parent IDs");

        ActionResult<Order> newOrderRes = await m_Service.PostOrder(order, parentId);
        if (newOrderRes.Result != null) return newOrderRes.Result;
        Order newOrder = newOrderRes.Value!;

        return CreatedAtAction(nameof(GetOrder), new { id = newOrder.OrderID }, newOrder);
    }

    // POST: api/Orders/ForceCloseAllChecks
    [HttpPost("ForceCloseAllChecks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostForceCloseAllChecks() =>
        await m_Service.ForceCloseAllChecks();

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(int id) =>
        await m_Service.DeleteOrder(id);
}
