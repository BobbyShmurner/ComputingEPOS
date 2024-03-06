using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputingEPOS.Models;
using ComputingEPOS.Backend.Services;
using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StockController : ControllerBase {
	private readonly IStockService m_Service;
	private readonly IOrdersService m_OrdersService;
	private readonly IOrderItemsService m_OrderItemsService;

    public StockController(IStockService service, IOrdersService ordersService, IOrderItemsService orderItemsService) {
		m_Service = service;
		m_OrdersService = ordersService;
		m_OrderItemsService = orderItemsService;
	}

	// GET: api/Stock
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<List<Stock>>> GetAllStock() =>
		await m_Service.GetAllStock();

	// GET: api/Stock/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Stock>> GetStock(int id) =>
		await m_Service.GetStock(id);

    // GET: api/Stock/5/Pmix
    [HttpGet("{id}/Pmix")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PmixReport>> GetStockPmix(int id, DateTime from, DateTime? to = null) =>
        await m_Service.GetStockPmix(id, from, to, m_OrdersService, m_OrderItemsService);

    // GET: api/Stock/Pmix
    [HttpGet("Pmix")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PmixReport>>> GetAllStockPmix(DateTime from, DateTime? to = null) =>
        await m_Service.GetAllStockPmix(from, to, m_OrdersService, m_OrderItemsService);

    // PUT: api/Stock/5
    [HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Stock>> PutStock(int id, Stock stock) {
		if (id != stock.StockID) return BadRequest(); 
		return await m_Service.PutStock(stock);
	}

	// POST: api/Stock
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<ActionResult<Stock>> PostStock(Stock stock) {
        ActionResult<Stock> newStockRes = await m_Service.PostStock(stock);
        if (newStockRes.Result != null) return newStockRes.Result;
        Stock newStock = newStockRes.Value!;

        return CreatedAtAction(nameof(GetStock), new { id = newStock.StockID }, newStock);
    }

	// POST: api/Stock/5/UpdateQuantity?delta=5
	[HttpPost("{id}/UpdateQuantity")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<ActionResult<Stock>> PostUpdateQuantity(int id, [Required] float delta) =>
		await m_Service.UpdateStockQuantity(id, delta);

	// DELETE: api/Stock/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteStock(int id) =>
		await m_Service.DeleteStock(id);
}
