using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Backend.Services;

public interface IStockService {
	public Task<ActionResult<List<Stock>>> GetAllStock();
	public Task<ActionResult<Stock>> GetStock(int id);
	public Task<ActionResult<PmixReport>> GetStockPmix(int id, DateTime? from, DateTime? to, IOrdersService ordersService, IOrderItemsService orderItemsService);
	public Task<ActionResult<List<PmixReport>>> GetAllStockPmix(DateTime? from, DateTime? to, IOrdersService ordersService, IOrderItemsService orderItemsService);

    public Task<ActionResult<Stock>> PutStock(Stock stock);

	public Task<ActionResult<Stock>> PostStock(Stock stock);
	public Task<ActionResult<Stock>> UpdateStockQuantity(int id, float delta);

	public Task<IActionResult> DeleteStock(int id);

	public Task<bool> StockExists(int id);
}
