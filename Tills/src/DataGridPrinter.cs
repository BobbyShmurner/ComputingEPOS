using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;

namespace ComputingEPOS.Tills;

public class DataGridPrinter<T> {
    public StringBuilder Sb { get; private set; } = new StringBuilder();
	public List<T> RowData { get; private set; }
	public List<DataGridColumnInfo> ColumnInfo { get; private set; }
	public double? PageWidth { get; private set; }

	public string? Title { get; private set; }
	public string ReportTitle => Title != null ? $"{Title} Report" : "Report";

	public List<string> ColumnHeaders { get; private set; }= new();
	public List<string[]> RowStrs { get; private set; } = new();

	public DataGridPrinter(List<T> rows, List<DataGridColumnInfo> columns, string? title = null, double? pageWidth = null) {
		Title = title;
		RowData = rows;
		ColumnInfo = columns;
		PageWidth = pageWidth;

		GenerateStringData();
	}

	public override string ToString() {
		Sb.Clear();
		List<int> columnWidths = ColumnHeaders.Select((header, i) => Math.Max(RowStrs.Max(row => row[i]?.Length ?? 0), header?.Length ?? 0)).ToList();

		// | Column 1 | Column 2 | Column 3 |
		// Column Widths + 3 for padding + 1 for the last column
		int totalWidth = columnWidths.Sum() + ColumnHeaders.Count * 3 + 1;

		Sb.AppendLine(Center(ReportTitle, totalWidth));
		Sb.AppendLine(new string(' ', totalWidth));

		// Top border
		for (int i = 0; i < ColumnHeaders.Count; i++) {
			if (i == 0) Sb.Append('┌');
			else Sb.Append("┬");

			for (int j = 0; j < columnWidths[i] + 2; j++) // +2 for padding
				Sb.Append('─');
		}
		Sb.AppendLine("┐");

		// Column headers
		for (int i = 0; i < ColumnHeaders.Count; i++) {
			Sb.Append("│ ");
			Sb.Append(Center(ColumnHeaders[i], columnWidths[i]));
			Sb.Append(' ');
		}
		Sb.AppendLine("│");

		// Header separator
		AppendHeaderSeperator(columnWidths);

		// Rows
		foreach (string[] row in RowStrs) {
			if (row.Length == 0 || row.All(string.IsNullOrEmpty)) {
				AppendHeaderSeperator(columnWidths);
				continue;
			}

			for (int i = 0; i < ColumnHeaders.Count; i++) {
				Sb.Append("│");
				Sb.Append(' ');
				Sb.Append(Center((i < row.Length ? row[i] : null) ?? "", columnWidths[i]));
				Sb.Append(' ');
			}

			Sb.AppendLine("│");
		}

		// Bottom border
		for (int i = 0; i < ColumnHeaders.Count; i++) {
			if (i == 0) Sb.Append('└');
			else Sb.Append("┴");

			for (int j = 0; j < columnWidths[i] + 2; j++) // +2 for padding
				Sb.Append('─');
		}
		Sb.AppendLine("┘");
	
		return Sb.ToString();
	}

	void AppendHeaderSeperator(List<int> columnWidths) {
		for (int i = 0; i < ColumnHeaders.Count; i++) {
			if (i == 0) Sb.Append('├');
			else Sb.Append("┼");

			for (int j = 0; j < columnWidths[i] + 2; j++) // +2 for padding
				Sb.Append('─');
		}
		Sb.AppendLine("┤");
	}

	public void Print() {
		string report = ToString();
		PrintManager.PrintString(report, ReportTitle, 16, new("Consolas"), orientation: PageOrientation.Landscape, pageWidth: PageWidth);
	}

	string Center(string text, int width) {
		int padding = width - text.Length;
		int leftPadding = padding / 2;
		int rightPadding = padding - leftPadding;

		return new string(' ', leftPadding) + text + new string(' ', rightPadding);
	}

	void GenerateStringData() {
		ColumnHeaders = ColumnInfo.Select(info => info.Header).ToList();
		RowStrs = RowData.Select(row =>
			ColumnInfo.Select(info =>
				string.Format(info.Format, row?.GetType().GetProperty(info.Binding)?.GetValue(row) ?? "")
			).ToArray()
		).ToList();
	}

	/// <summary>
	/// Insert a row at the specified index
	/// Leave the row empty to insert a horizontal line
	/// </summary>
	/// <param name="index">The index to insert the row at</param>
	/// <param name="row">The row to insert. This length must match the number of columns. Insert an empty array to insert a horizontal line</param>
	public void InsertArbitraryRow(int index, string[] row) {
		RowStrs.Insert(index, row);
	}

	/// <summary>
	/// Appends a row to the end of the grid
	/// Leave the row empty to insert a horizontal line
	/// </summary>
	/// <param name="row">The row to insert. This length must match the number of columns. Insert an empty array to insert a horizontal line</param>
	public void AppendArbitraryRow(string[] row) {
		RowStrs.Add(row);
	}
}