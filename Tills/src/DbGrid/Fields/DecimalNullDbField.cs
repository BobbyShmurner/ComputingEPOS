using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class DecimalNullDbField<T> : TextBoxDbField<T, decimal?> {
    public DecimalNullDbField(string label, string fieldName) : base(label, fieldName) { }
    protected override decimal? FromString(string? data) {
        if (data == null || data == "")
            return null;

        if (decimal.TryParse(data, out decimal result))
            return result;

        return null;
    }
}
