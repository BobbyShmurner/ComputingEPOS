using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class BoolDbField<T> : BaseDbField<T, bool> {
    public CheckBox CheckBox { get; private set; }
    protected override FrameworkElement GetElement() => CheckBox;

    public BoolDbField(string label, string fieldName) : base(label, fieldName) {
        CheckBox = new CheckBox {
            FontSize = 16,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

    protected override bool GetData() => CheckBox.IsChecked == true;
    protected override void SetData(bool data) => CheckBox.IsChecked = data;
}
