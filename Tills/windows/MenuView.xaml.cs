using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

public partial class MenuView : UserControl
{
    public const int SCROLL_AMOUNT = 100;

    public OrderManager OrderManager { get; private set; }
    public OrderMenuManager OrderMenuManager { get; private set; }

    public MenuView()
    {
        InitializeComponent();
        this.DataContext = this;

        OrderMenuManager = OrderMenuManager.CreateTestMenus(this);
        OrderManager = new(this);
    }

    #region ShowScrollButtons Dependency Property
    public static readonly DependencyProperty ShowScrollButtonsProperty = DependencyProperty.Register("ShowScrollButtons", typeof(Visibility), typeof(MenuView), new UIPropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnShowScrollButtonsChanged), new CoerceValueCallback(OnCoerceShowScrollButtons)));

    private static object OnCoerceShowScrollButtons(DependencyObject o, object value)
    {
        if (o == null) return value;
        MenuView menuView = (MenuView)o;
        return menuView.OnCoerceShowScrollButtons((Visibility)value);
    }

    private static void OnShowScrollButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o == null) return;
        MenuView menuView = (MenuView)o;

        menuView.OnShowScrollButtonsChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
    }

    protected virtual Visibility OnCoerceShowScrollButtons(Visibility value)
    {
        return value;
    }

    protected virtual void OnShowScrollButtonsChanged(Visibility oldValue, Visibility newValue)
    {
        OrderManager.Selected?.BringIntoView();
    }

    public Visibility ShowScrollButtons
    {
        get => (Visibility)GetValue(ShowScrollButtonsProperty);
        set => SetValue(ShowScrollButtonsProperty, value);
    }

    private void SV_Orders_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ShowScrollButtons = e.ViewportHeight > e.ExtentHeight ? Visibility.Collapsed : Visibility.Visible;
    }

    #endregion

    private void Orders_SV_Up(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset - SCROLL_AMOUNT));
    private void Orders_SV_Down(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset + SCROLL_AMOUNT));

    private void Button_Clear(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.DeleteAllItems(true); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.RemoveSelectedOrderItem(true); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void SitInButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.CheckoutOrder(CheckoutType.SitIn); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void TakeAwayButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.CheckoutOrder(CheckoutType.TakeAway); }
        finally { UIDispatcher.UpdateUI(); }
    });


    private void OrderItemsEmptyFillButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() => {
        try { OrderManager.DeselectItem(); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void TransactionButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        decimal? amount = null;
        Transaction.PaymentMethods paymentMethod = Transaction.PaymentMethods.Cash;
        TransactionButton.SpecialFunctions specialFunction = TransactionButton.SpecialFunctions.None;

        UIDispatcher.DispatchOnUIThread(() => {
            amount = TransactionButton.GetAmount((UIElement)sender);
            paymentMethod = TransactionButton.GetPaymentMethod((UIElement)sender);
            specialFunction = TransactionButton.GetSpecial((UIElement)sender);
        });

        try { await OrderManager.PayForOrder(amount, paymentMethod, specialFunction); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void ComboButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        try { await OrderManager.MakeSelectedCombo(); }
        finally { UIDispatcher.UpdateUI(); }
    });

    private void ModifyButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void FunctionsButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
    {
        OrderMenuManager.ShowFunctionsScreen();
        UIDispatcher.UpdateUI();
    });

    private void PowerOffButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.DispatchOnUIThread(() => Application.Current.Shutdown(0))
    );

    private void RebootButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(() =>
        UIDispatcher.DispatchOnUIThread(() => {
            Restart();
        })
    );

    private void ReportsButton_Click(object sender, RoutedEventArgs e) => UIDispatcher.EnqueueUIAction(async () => {
        UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
            MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.ReportsView);
        });
        await MainWindow.Instance.ReportsView.RefreshCurrentGrid();
        UIDispatcher.UpdateUI();
    });

    private void UpsizeButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void DownsizeButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void LogoutButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();
    private void NotImplementedButton_Click(object sender, RoutedEventArgs e) => Modal.Instance.ShowNotImplementedModal();

    /// <summary>
    /// Restarts the application. <br/>
    /// Taken From WinForms Source: https://github.com/dotnet/winforms/blob/bd97476fa596ac2063153181daa6a46251dc755d/src/System.Windows.Forms/src/System/Windows/Forms/Application.cs#L1090-L1127
    /// </summary>
    public static void Restart()
    {
        if (Assembly.GetEntryAssembly() is null)
        {
            throw new NotSupportedException();
        }

        bool hrefExeCase = false;

        Process process = Process.GetCurrentProcess();
        Debug.Assert(process is not null);

        if (!hrefExeCase)
        {
            // Regular app case
            string[] arguments = Environment.GetCommandLineArgs();
            Debug.Assert(arguments is not null && arguments.Length > 0);

            ProcessStartInfo currentStartInfo = new();
            currentStartInfo.FileName = Environment.ProcessPath;
            if (arguments.Length >= 2)
            {
                StringBuilder sb = new((arguments.Length - 1) * 16);
                for (int argumentIndex = 1; argumentIndex < arguments.Length; argumentIndex++)
                {
                    sb.Append($"\"{arguments[argumentIndex]}\" ");
                }

                currentStartInfo.Arguments = sb.ToString(0, sb.Length - 1);
            }

            Application.Current.Shutdown();
            Process.Start(currentStartInfo);
        }
    }
}
