using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class FloatDbField<T> : TextBoxDbField<T, float> {
    public FloatDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override float FromString(string? data) => float.Parse(data!);
}
