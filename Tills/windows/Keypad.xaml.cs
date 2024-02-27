using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for Keypad.xaml
/// </summary>
public partial class Keypad : UserControl
{
    public event EventHandler<int>? NumPressed;
    public event EventHandler? Confirm;
    public event EventHandler? Clear;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(int), typeof(Keypad), new FrameworkPropertyMetadata(0));
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Keypad() {
        InitializeComponent();
        DataContext = this;
    }

    public void ClearVaule() => Value = 0;

    private void Num_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
    {
        string content = ((Button)sender).Content.ToString()!;
        int num = int.Parse(content);

        Value = int.Parse(Value.ToString() + content);
        NumPressed?.Invoke(this, num);
    });

    private void Confirm_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => 
        Confirm?.Invoke(this, EventArgs.Empty)
    );

    private void Clear_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => 
    {
        ClearVaule();
        Clear?.Invoke(this, EventArgs.Empty);
    });
}
