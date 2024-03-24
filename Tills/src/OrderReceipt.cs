using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using static ComputingEPOS.Models.Transaction;

namespace ComputingEPOS.Tills;

public class OrderReceipt {
    public OrderManager OrderManager { get; private set; }
    public int Width { get; set; }
    public StringBuilder Sb { get; private set; } = new StringBuilder();

    public OrderReceipt(OrderManager orderManager, int width) {
        OrderManager = orderManager;
        Width = width;
    }

    public override string ToString() => PrintReceipt();

    public string PrintReceipt() {
        AppendHeading($"Order #{OrderManager.OrderNumber:00}", top: true);
        AppendBlankLine();
        AppendLine($"Cashier: ", OrderManager.Cashier);

        DateTime date = OrderManager.CurrentOrder?.OrderDuration != null ? OrderManager.CurrentOrder.Date.AddSeconds(OrderManager.CurrentOrder.OrderDuration.Value) : DateTime.Now;
        AppendLine($"Date: ", date.ToString("dd/MM/yy"));
        AppendLine($"Time: ", date.ToString("hh:mm tt"));

        AppendBlankLine();

        AppendHeading("Items");
        FormatOrderItems();

        AppendHeading("Payments");
        FormatPayments();

        AppendHeading();
        FormatEndMessage();
        AppendHeading(bottom: true);

        return Sb.ToString();
    }

    string TruncateLine(string line, string suffix = "") {
        if (line.Length + suffix.Length > Width) {
            line = line.Substring(0, Width - suffix.Length - 3) + "...";
        } else {
            line += new string(' ', Width - line.Length - suffix.Length);
        }

        return line + suffix;
    }

    void FormatOrderItems() {
        foreach(var item in OrderManager.AllItems) {
            string price = item.Price.HasValue ? $" £{item.Price.Value:n2}" : "";

            string line = string.Empty;

            for (int i = 0; i < item.IndentLevel; i++)
                line += "  ";

            line += $"- {item.Text}";

            if (item.IndentLevel == 0)
                AppendBlankLine();
            AppendLine(line, suffix: price);
        }

        AppendBlankLine();
    }

    void FormatPayments() {
        AppendBlankLine();

        if (OrderManager.Transactions.Count == 0) {
            AppendCenterLine("No Payments Made!");
            AppendBlankLine();
        } else {
            List<Transaction> transactions = OrderManager.Transactions
                .Where(t => t.Method != "Cash")
                .Append(
                    new Transaction {
                        AmountPaid = OrderManager.Transactions.Where(t => t.Method == "Cash").Sum(t => t.AmountPaid),
                        Method = "Cash"
                    }
                )
                .ToList();

            foreach(var transaction in transactions) {
                string amount = $" £{transaction.AmountPaid:n2}";
                string method = $"- {transaction.Method ?? "Unknown"} Payment";

                AppendLine(method, suffix: amount);
                AppendBlankLine();
            }
        }

        AppendLine("Subtotal", suffix: $" £{OrderManager.SubTotal:n2}");
        AppendLine("Tax", suffix: $" £{OrderManager.Tax:n2}");

        AppendBlankLine();

        AppendLine("Total", suffix: $" £{OrderManager.Total:n2}");
        if (OrderManager.Outstanding > 0)
            AppendLine("Outstanding", suffix: $" £{OrderManager.Outstanding:n2}");

        AppendBlankLine();
    }

    void FormatEndMessage() {
        AppendBlankLine();

        if (OrderManager.Outstanding > 0) {
            AppendCenterLine("Please Pay For The Remainder");
            AppendCenterLine("Of Your Order!");
        } else {
            AppendCenterLine("Thank You For Your Purchase!");
            AppendCenterLine("Please Come Again!");
        }

        AppendBlankLine();
    }

    void AppendBlankLine() {
        AppendLine(new string(' ', Width));
    }

    void AppendCenterLine(string line, string suffix = "") {
        AppendLine(new string(' ', (int)Math.Floor((Width - line.Length) / 2f)) + line + new string(' ', (int)Math.Ceiling((Width - line.Length) / 2f)), suffix);
    }

    void AppendLine(string line, string suffix = "") {
        Sb.Append("│ ");
        Sb.Append(TruncateLine(line, suffix));
        Sb.AppendLine(" │");
    }

    void AppendHeading(string? heading = null, bool top = false, bool bottom = false) {
        if (top) Sb.Append("┌");
        else if (bottom) Sb.Append("└");
        else Sb.Append("├");

        if (heading == null) {
            Sb.Append(new string('─', Width + 2));
        } else {
            float halfExtent = (Width - heading.Length) / 2f;

            Sb.Append(new string('─', (int)Math.Floor(halfExtent)));
            Sb.Append(" ");
            Sb.Append(heading);
            Sb.Append(" ");
            Sb.Append(new string('─', (int)Math.Ceiling(halfExtent)));
        }
        
        if (top) Sb.AppendLine("┐");
        else if (bottom) Sb.AppendLine("┘");
        else Sb.AppendLine("┤");
    }
}