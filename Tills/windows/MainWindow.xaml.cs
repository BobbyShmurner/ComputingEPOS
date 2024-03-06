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
using System.Diagnostics;
using System.Windows.Threading;
using System.Reflection;
using System.ComponentModel;

namespace ComputingEPOS.Tills {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public const int SCROLL_AMOUNT = 100;

        public TimeDisplay Time { get; private set; }
        public ViewManager RootViewManager { get; private set; }

        #pragma warning disable CS8618
        public static MainWindow Instance;
        #pragma warning restore CS8618

        public MainWindow() {
            Instance = this;
            InitializeComponent();

            Time = new TimeDisplay();
            DataContext = this;
            Width = 1200;
            Height = 900;

            RootViewManager = new(Grid_MainViewContainer);

            RootViewManager.ShowView(ConnectionScreen);
            Modal.Instance.Show("Connecting...", false);

            Task.Run(async () => {
                await ConnectionScreen.EnsureConnected(false, false);
                await MenuView.OrderManager.NextOrder();

                UIDispatcher.EnqueueAndUpdateOnUIThread(() => {
                    Modal.Instance.Hide();
                    RootViewManager.ShowView(MenuView);
                });
            });
        }

        #region ScaleValue Dependency Property
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(MainWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            if (o == null) return value;
            MainWindow mainWindow = (MainWindow)o;
            return mainWindow.OnCoerceScaleValue((double)value);
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o == null) return;
            MainWindow mainWindow = (MainWindow)o;

            mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value)) return 1.0f;
            return Math.Max(0.1, value);
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue) { }

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }
        #endregion

        private void Root_SizeChanged(object sender, EventArgs e) => CalculateScale();

        private void CalculateScale()
        {
            double xScale = ActualWidth / 800f;
            double yScale = ActualHeight / 600f;
            double value = Math.Min(xScale, yScale);

            ScaleValue = (double)OnCoerceScaleValue(mainWindow, value);
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            Shutdown();
        }

        public static void Shutdown(bool restart = false, int exitCode = 0) {
            UIDispatcher.DispatchOnUIThread(() => {
                var menuView = Instance.MenuView;
                Modal.Instance.Show("Shutting down...", false);

                Task.Run(async () => {
                    try {
                        if (menuView.OrderManager.CurrentOrder != null) await menuView.OrderManager.DeleteCurrentOrder();
                        await menuView.OrderManager.CloseAllPaidChecks();

                        if (restart) Restart();
                        UIDispatcher.DispatchOnUIThread(() => Application.Current.Shutdown(exitCode));
                    } catch (Exception ex) {
                        Trace.WriteLine(ex);
                        throw;
                    }
                });
            });
        }

        /// <summary>
        /// Restarts the application. <br/>
        /// Taken From WinForms Source: https://github.com/dotnet/winforms/blob/bd97476fa596ac2063153181daa6a46251dc755d/src/System.Windows.Forms/src/System/Windows/Forms/Application.cs#L1090-L1127
        /// </summary>
        static void Restart() {
            if (Assembly.GetEntryAssembly() is null) {
                throw new NotSupportedException();
            }

            Process process = Process.GetCurrentProcess();
            Debug.Assert(process is not null);

            
            string[] arguments = Environment.GetCommandLineArgs();
            Debug.Assert(arguments is not null && arguments.Length > 0);

            ProcessStartInfo currentStartInfo = new();
            currentStartInfo.FileName = Environment.ProcessPath;
            if (arguments.Length >= 2) {
                StringBuilder sb = new((arguments.Length - 1) * 16);
                for (int argumentIndex = 1; argumentIndex < arguments.Length; argumentIndex++) {
                    sb.Append($"\"{arguments[argumentIndex]}\" ");
                }

                currentStartInfo.Arguments = sb.ToString(0, sb.Length - 1);
            }

                
            Process.Start(currentStartInfo);
        }
    }
}
