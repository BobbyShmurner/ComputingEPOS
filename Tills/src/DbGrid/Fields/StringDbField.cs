using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class StringDbField<T> : TextBoxDbField<T, string> {
    public StringDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override string FromString(string? data) => data ?? "";
}
