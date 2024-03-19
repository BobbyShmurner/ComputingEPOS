using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public abstract class BaseDbField<T, U> : IDbField {
    public string Label { get; private set; }
    public string FieldName { get; private set; }

    public Type Type => typeof(T);

    public BaseDbField(string label, string fieldName) {
        Label = label;
        FieldName = fieldName;
    }

    public FrameworkElement Display() {
        DockPanel panel = new DockPanel {
            Margin = new Thickness(5)
        };

        Label label = new Label {
            FontSize = 16,
            Content = Label
        };

        var element = GetElement();

        panel.Children.Add(label);
        panel.Children.Add(element);

        return panel;
    }

    protected abstract FrameworkElement GetElement();
    protected abstract void SetData(U data);
    protected abstract U GetData();

    public void SetData(object data) {
        if (!(data is T)) throw new ArgumentException("Invalid data type");

        SetData((U)Type.GetProperty(FieldName)!.GetValue(data)!);
    }

    public void UpdateData(ref object data) {
        if (!(data is T)) throw new ArgumentException("Invalid data type");
        Type.GetProperty(FieldName)!.SetValue(data, GetData());
    }
}
