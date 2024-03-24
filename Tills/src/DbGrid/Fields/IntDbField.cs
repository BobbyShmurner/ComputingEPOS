using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class IntDbField<T> : TextBoxDbField<T, int> {
    public IntDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override int FromString(string? data) => int.Parse(data!);
}
