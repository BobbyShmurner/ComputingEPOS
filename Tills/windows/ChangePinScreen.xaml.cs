using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
using ComputingEPOS.Models;

namespace ComputingEPOS.Tills;

/// <summary>
/// Interaction logic for ChangePinScreen.xaml
/// </summary>
public partial class ChangePinScreen : UserControl {
    public static ChangePinScreen Instance => MainWindow.Instance.ChangePinScreen;
    static bool IsManager => LoginManager.Instance.CurrentEmployee?.Role == Employee.Roles.Manager;

    Employee? m_SelectedEmployee = null;
    public Employee? SeletcedEmployee { 
        get => m_SelectedEmployee;
        set {
            m_SelectedEmployee = value;
            SetValue(SelectedEmployeeNameProperty, SelectedEmployeeName);
        }
    }

    public static readonly DependencyProperty SelectedEmployeeNameProperty = DependencyProperty.RegisterAttached(
        nameof(SelectedEmployeeName), typeof(string), typeof(ChangePinScreen), new FrameworkPropertyMetadata(null)
    );

    public static readonly DependencyProperty IsManagerProperty = DependencyProperty.RegisterAttached(
        nameof(IsManager), typeof(bool), typeof(ChangePinScreen), new FrameworkPropertyMetadata(false)
    );

    public static readonly DependencyProperty EmployeeColumnWidthProperty = DependencyProperty.RegisterAttached(
        "EmployeeColumnWidth", typeof(GridLength), typeof(ChangePinScreen), new FrameworkPropertyMetadata(new GridLength(0))
    );

    public static readonly DependencyProperty PinPassTextDependencyProperty = DependencyProperty.Register(
        nameof(PinPassText), typeof(string), typeof(ChangePinScreen), new FrameworkPropertyMetadata(default(string))
    );

    public static readonly DependencyProperty StatusTextProperty = DependencyProperty.RegisterAttached(
        nameof(StatusText), typeof(string), typeof(ChangePinScreen), new FrameworkPropertyMetadata(default(string))
    );

    public string SelectedEmployeeName => SeletcedEmployee == null ? "None" : $"{SeletcedEmployee.FirstNames} {SeletcedEmployee.LastName}";

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

    public string StatusText {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    string currentPin = "";
    string newPin = "";

    public ChangePinScreen() {
        InitializeComponent();
        DataContext = this;

        Keypad.Clear += (_, _) => PinPass = "";
        Keypad.NumPressed += (_, num) => PinPass += num.ToString();
        Keypad.Confirm += (_, _) => {
            OnKeypadConfirm();
            UIDispatcher.UpdateUI();
        };
    }

    void ResetInternalState() {
        SeletcedEmployee = LoginManager.Instance.CurrentEmployee;
        Keypad.ClearValue();

        currentPin = "";
        newPin = "";
        StatusText = "Please Enter Your Current Pin:";

        SetValue(IsManagerProperty, IsManager);
        SetValue(EmployeeColumnWidthProperty, IsManager ? new GridLength(3, GridUnitType.Star) : new GridLength(0));
    }

    void OnKeypadConfirm() {
        if (SeletcedEmployee == null) {
            UIDispatcher.EnqueueOnUIThread(() => {
                ResetInternalState();
                Modal.Instance.Show("Please Select An Employee!");
            });

            return;
        }

        if (currentPin == "") {
            currentPin = PinPass;
            Keypad.ClearValue();

            UIDispatcher.EnqueueOnUIThread(() => StatusText = "Please Enter Your New Pin:");
            return;
        }

        newPin = PinPass;
        Keypad.ClearValue();

        UIDispatcher.EnqueueUIAction(async () => await ChangePin(SeletcedEmployee, currentPin, newPin));
    }

    async Task ChangePin(Employee employee, string currentPin, string newPin) {
        try {
            UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Show("Changing Pin...", false));
            await Api.Employees.UpdatePin(employee, currentPin, newPin);

            Back();
            UIDispatcher.EnqueueOnUIThread(() => {
                Modal.Instance.Show("Successfully Changed Pin!");
            });

        } finally {
            UIDispatcher.EnqueueAndUpdateOnUIThread(() => ResetInternalState());
        }
    }

    async Task PopulateEmployeeButtons() {
        List<Employee> employees = await Api.Employees.GetEmployees();

        UIDispatcher.EnqueueOnUIThread(() => {
            EmployeePanel.Children.Clear();

            foreach (Employee employee in employees) {
                TextBlock textBlock = new() {
                    Text = $"{employee.FirstNames} {employee.LastName}",
                };

                Button button = new() { Content = textBlock };
                ChangePinEmployeeButton.SetEmployee(button, employee);

                button.Click += (sender, e) => {
                    SeletcedEmployee = ChangePinEmployeeButton.GetEmployee(button);
                };

                EmployeePanel.Children.Add(button);
            }
        });
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(Back);

    void Back() =>
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView));

    public async Task Show() {
        bool isManager = false;

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            ResetInternalState();
            EmployeePanel.Children.Clear();
            isManager = IsManager;

            Modal.Instance.Show("Loading...", false);
            MainWindow.Instance.RootViewManager.ShowView(this);
        });

        if (isManager) await PopulateEmployeeButtons();

        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            Modal.Instance.Hide();
        });
    }
}
