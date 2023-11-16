using Microsoft.AspNetCore.Mvc;
using ComputingEPOS.Backend.Models;

namespace ComputingEPOS.Backend.Services;

public interface IStockService {
	public Task<ActionResult<List<Stock>>> GetAllStock();
	public Task<ActionResult<Stock>> GetStock(int id);

	public Task<ActionResult<Stock>> PutStock(Stock stock);

	public Task<ActionResult<Stock>> PostStock(Stock stock);
	public Task<ActionResult<Stock>> UpdateStockQuantity(int id, float delta);

	public Task<IActionResult> DeleteStock(int id);

	public Task<bool> StockExists(int id);
}
