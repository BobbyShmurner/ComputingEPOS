﻿using System;
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

namespace ComputingEPOS.Tills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int SCROLL_AMOUNT = 100;

        public OrderManager OrderManager { get; private set; }
        public TimeDisplay Time { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Time = new TimeDisplay();
            this.DataContext = this;

            var badItem = new OrderListItem("Bad :(");
            var goodItem = new OrderListItem("Good :)");

            var addCheeseItem = new OrderListItem("Add Cheese", 0.2M);
            var addBadCheeseItem = addCheeseItem.NewFrom(badItem);
            var addGoodCheeseItem = addCheeseItem.NewFrom(goodItem);

            var testItem = new OrderListItem("Test Burger", 6.9M);
            var testItemCheese = testItem.NewFrom(addCheeseItem);
            var testItemGood = testItem.NewFrom(goodItem);
            var testItemWorst = testItem.NewFrom(badItem, addBadCheeseItem);

            MenuButton?[,] menuItems = {
                { new BasicMenuButton(testItem, "Test Item"), new SubItemMenuButton(addCheeseItem, "Cheese") },
                { new BasicMenuButton(testItemGood, "Test Item But Better"), new SubItemMenuButton(addGoodCheeseItem, "Cheese But Better") },
                { new BasicMenuButton(testItemCheese, "Test Item With Cheese"), null },
                { new BasicMenuButton(testItemWorst, "Test Item But Worse :("), new SubItemMenuButton(addBadCheeseItem, "Cheese But Worse :(") },
            };

            Menu testMenu = new Menu("Test", menuItems, 5, 5);

            var burgerItem = new OrderListItem("Burger", 5.99M);

            MenuButton?[,] burgerMenuItems = {
                { new BasicMenuButton(burgerItem, "Burger :)") },
            };

            OrderManager = new(this);
            MenuManager menuManager = new(this, OrderManager, testMenu);
            Menu burgerMenu = menuManager.CreateMenu("Burgers", burgerMenuItems);
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

        #region ShowScrollButtons Dependency Property
        public static readonly DependencyProperty ShowScrollButtonsProperty = DependencyProperty.Register("ShowScrollButtons", typeof(Visibility), typeof(MainWindow), new UIPropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnShowScrollButtonsChanged), new CoerceValueCallback(OnCoerceShowScrollButtons)));

        private static object OnCoerceShowScrollButtons(DependencyObject o, object value)
        {
            if (o == null) return value;
            MainWindow mainWindow = (MainWindow)o;
            return mainWindow.OnCoerceShowScrollButtons((Visibility)value);
        }

        private static void OnShowScrollButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o == null) return;
            MainWindow mainWindow = (MainWindow)o;

            mainWindow.OnShowScrollButtonsChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
        }

        protected virtual Visibility OnCoerceShowScrollButtons(Visibility value)
        {
            return value;
        }

        protected virtual void OnShowScrollButtonsChanged(Visibility oldValue, Visibility newValue) {
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

        public static readonly DependencyProperty EnableButtonsWhenItemSelectedProperty = DependencyProperty.Register("EnableButtonsWhenItemSelected", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));
        public bool EnableButtonsWhenItemSelected
        {
            get => (bool)GetValue(EnableButtonsWhenItemSelectedProperty);
            set => SetValue(EnableButtonsWhenItemSelectedProperty, value);
        }

        private void Root_SizeChanged(object sender, EventArgs e) => CalculateScale();

        private void CalculateScale()
        {
            double yScale = ActualHeight / 600f;
            double xScale = ActualWidth / 800f;
            double value = Math.Min(xScale, yScale);

            ScaleValue = (double)OnCoerceScaleValue(mainWindow, value);
        }

        private void Orders_SV_Up(object sender, RoutedEventArgs e) => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset - SCROLL_AMOUNT);
        private void Orders_SV_Down(object sender, RoutedEventArgs e) => SV_Orders.ScrollToVerticalOffset(SV_Orders.VerticalOffset + SCROLL_AMOUNT);

        private void Button_Clear(object sender, RoutedEventArgs e) => OrderManager.DeleteAll();

        private void DeleteButton_Click(object sender, RoutedEventArgs e) => OrderManager.RemoveSelectedOrder();
        private void ModifyButton_Click(object sender, RoutedEventArgs e) { }

        private void OrderItemsEmptyFillButton_Click(object sender, RoutedEventArgs e) => OrderManager.DeselectItem();
    }
}
