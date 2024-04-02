using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Threading;

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for LoginScreen.xaml
/// </summary>
public partial class LoginScreen : UserControl {
    public static LoginScreen Instance => MainWindow.Instance.LoginScreen;
    public LoginManager Manager => LoginManager.Instance;

    public static readonly DependencyProperty PinPassTextDependencyProperty = DependencyProperty.Register(
        nameof(PinPassText), typeof(string), typeof(LoginScreen), new FrameworkPropertyMetadata(default(string))
    );

    string m_PinPass = string.Empty;
    public string PinPass {
        get => m_PinPass;
        set {
            m_PinPass = value;
            SetValue(PinPassTextDependencyProperty, new string('*', value.Length));
        }
    }

    public string PinPassText {
        get => (string)GetValue(PinPassTextDependencyProperty);
    }

    public LoginScreen() {
        InitializeComponent();
        DataContext = this;

        Keypad.NumPressed += Keypad_NumPressed;
        Keypad.Confirm += Keypad_Confirm;
        Keypad.Clear += Keypad_Clear;
    }

    private void Keypad_Confirm(object? sender, EventArgs e) {
        string pin = PinPass;
        Keypad.ClearValue();

        if (pin.Length == 0) {
            Modal.Instance.Show("Please Enter A Pin!");
            return;
        }

        UIDispatcher.EnqueueUIAction(async () => {
            await LoginManager.Instance.Login(pin);
            UIDispatcher.UpdateUI();
        });
    }

    private void Keypad_Clear(object? sender, EventArgs e) {
        PinPass = "";
    }

    private void Keypad_NumPressed(object? sender, int num) {
        PinPass += num.ToString();
    }
}
