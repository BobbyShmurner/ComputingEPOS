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
using ComputingEPOS.Tills.Api;
using System.Windows.Threading;

namespace ComputingEPOS.Tills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int SCROLL_AMOUNT = 100;

        public TimeDisplay Time { get; private set; }

        public ViewManager RootViewManager { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Time = new TimeDisplay();
            this.DataContext = this;

            RootViewManager = new(Grid_MainViewContainer);

            this.Width = 1200;
            this.Height = 900;

            

            ConnectionScreen.Ping();
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
    }
}
