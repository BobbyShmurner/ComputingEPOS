using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public class EnumDbField<T, TEnum> : BaseDbField<T, TEnum>, INotifyPropertyChanged where TEnum : struct, Enum {
    public event PropertyChangedEventHandler? PropertyChanged;
    public ComboBox ComboBox { get; private set; }
    protected override FrameworkElement GetElement() => ComboBox;

    public TEnum? Value {
        get => (TEnum?)ComboBox.SelectedItem;
        set => ComboBox.SelectedItem = value;
    }

    public EnumDbField(string label, string fieldName) : base(label, fieldName) {
        ComboBox = new ComboBox {
            VerticalContentAlignment = VerticalAlignment.Center,
            ItemsSource = Enum.GetValues(typeof(TEnum)).Cast<TEnum>(),
            FontSize = 16,
            SelectedItem = null
        };

        Value = Value;
    }

    protected override TEnum GetData() => Value ?? default;
    protected override void SetData(TEnum data) => Value = data;
}
