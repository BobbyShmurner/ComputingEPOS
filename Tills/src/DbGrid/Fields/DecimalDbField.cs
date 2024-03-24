using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class DecimalDbField<T> : TextBoxDbField<T, decimal> {
    public DecimalDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override decimal FromString(string? data) => decimal.Parse(data!);
}
