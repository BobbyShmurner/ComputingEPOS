using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Common.Models;

public class PmixReport {
	public Stock Stock { get; set; } = new Stock();

	public float QuantitySold { get; set; }

	public decimal Gross { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime From { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime To { get; set; }
}