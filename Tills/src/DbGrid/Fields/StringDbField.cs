using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class StringDbField<T> : BaseDbField<T, string> {
    public TextBox TextBox { get; private set; }
    protected override FrameworkElement GetElement() => TextBox;

    public StringDbField(string label, string fieldName) : base(label, fieldName) {
        TextBox = new TextBox {
            FontSize = 16,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

    protected override string GetData() =>
        TextBox.Text.Trim();

    protected override void SetData(string? data) =>
        TextBox.Text = data ?? "";
}
