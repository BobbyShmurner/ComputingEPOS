using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class IntNullDbField<T> : TextBoxDbField<T, int?> {
    public IntNullDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override int? FromString(string? data) {
        if (data == null || data == "")
            return null;

        if (int.TryParse(data, out int result))
            return result;

        return null;
    }
}
