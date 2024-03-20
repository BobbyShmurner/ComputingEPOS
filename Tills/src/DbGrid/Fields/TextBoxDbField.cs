using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public abstract class TextBoxDbField<T, U> : BaseDbField<T, U> {
    public TextBox TextBox { get; private set; }
    protected override FrameworkElement GetElement() => TextBox;

    public TextBoxDbField(string label, string fieldName) : base(label, fieldName) {
        TextBox = new TextBox {
            FontSize = 16,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

    protected abstract U FromString(string? data);
    protected virtual string ToString(U? data) => data?.ToString() ?? "";

    protected override U GetData() =>
        FromString(TextBox.Text.Trim());

    protected override void SetData(U? data) =>
        TextBox.Text = ToString(data);
}
