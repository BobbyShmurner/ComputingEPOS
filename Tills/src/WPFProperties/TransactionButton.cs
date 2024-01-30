using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills;

public static class TransactionButton
{
    public static readonly DependencyProperty AmountProperty = DependencyProperty.RegisterAttached("Amount",
            typeof(decimal), typeof(TransactionButton), new FrameworkPropertyMetadata(null));

    public static decimal GetAmount(UIElement element)
    {
        if (element == null)
            throw new ArgumentNullException("element");
        return (decimal)element.GetValue(AmountProperty);
    }

    public static void SetAmount(UIElement element, decimal value)
    {
        if (element == null)
            throw new ArgumentNullException("element");
        element.SetValue(AmountProperty, value);
    }

    public static readonly DependencyProperty PaymentMethodProperty = DependencyProperty.RegisterAttached("PaymentMethod",
            typeof(Transaction.PaymentMethods), typeof(TransactionButton), new FrameworkPropertyMetadata(null));

    public static Transaction.PaymentMethods GetPaymentMethod(UIElement element)
    {
        if (element == null)
            throw new ArgumentNullException("element");
        return (Transaction.PaymentMethods)element.GetValue(PaymentMethodProperty);
    }

    public static void SetPaymentMethod(UIElement element, Transaction.PaymentMethods value)
    {
        if (element == null)
            throw new ArgumentNullException("element");
        element.SetValue(PaymentMethodProperty, value);
    }
}
