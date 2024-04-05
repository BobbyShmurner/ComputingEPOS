using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills;

public static class ChangePinEmployeeButton {
    public static readonly DependencyProperty EmployeeProperty = DependencyProperty.RegisterAttached("Employee",
            typeof(Employee), typeof(ChangePinEmployeeButton), new FrameworkPropertyMetadata(null));

    public static Employee GetEmployee(UIElement element) {
        if (element == null) throw new ArgumentNullException("element");
        return (Employee)element.GetValue(EmployeeProperty);
    }

    public static void SetEmployee(UIElement element, Employee value) {
        if (element == null) throw new ArgumentNullException("element");
        element.SetValue(EmployeeProperty, value);
    }
}
