using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ComputingEPOS.Tills; 

public static class PrintManager {
    public static void PrintString(string content, string caption, double fontSize = 12, FontFamily? fontFamily = null, Thickness? padding = null, PageOrientation orientation = PageOrientation.Portrait) {
        var dialog = new PrintDialog();
        dialog.PrintTicket.PageOrientation = orientation;

        var run = new Run(content) {
            FontSize = fontSize,
            FontFamily = fontFamily ?? new FontFamily("Arial"),
        };

        var paragraph = new Paragraph(run);

        var doc = new FlowDocument(paragraph) {
            PagePadding = padding ?? new Thickness(100),
            PageWidth = dialog.PrintableAreaWidth,
            PageHeight = dialog.PrintableAreaHeight,
            ColumnWidth = dialog.PrintableAreaWidth,
            IsColumnWidthFlexible = true,
        };

        dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, caption);
    }
}
