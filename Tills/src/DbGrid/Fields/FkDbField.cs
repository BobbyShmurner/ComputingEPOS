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

public class FkDbField<T> : BaseDbField<T, int> {
    public ComboBox ComboBox { get; private set; }
    protected override FrameworkElement GetElement() => ComboBox;

    List<(int, string)> KeysAndDisplayNames = new();
    List<string> DisplayNames => KeysAndDisplayNames.Select(x => x.Item2).ToList();

    string GetDisplayName(int key) => KeysAndDisplayNames.FirstOrDefault(x => x.Item1 == key).Item2;
    int GetKey(string displayName) => KeysAndDisplayNames.FirstOrDefault(x => x.Item2 == displayName).Item1;

    public FkDbField(string label, string fieldName, List<(int, string)> keysAndDisplayNames) : base(label, fieldName) {
        KeysAndDisplayNames = keysAndDisplayNames;

        ComboBox = new ComboBox {
            VerticalContentAlignment = VerticalAlignment.Center,
            ItemsSource = DisplayNames,
            FontSize = 16,
            SelectedItem = null
        };
    }

    protected override int GetData() => GetKey((string)ComboBox.SelectedItem);
    protected override void SetData(int data) => ComboBox.SelectedItem = GetDisplayName(data);
}
