using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace ComputingEPOS.Tills;

public class ButtonView : IMainView
{
    public MainWindow Window { get; private set; }
    public Grid Grid { get; private set; }

    public UIElement ViewElement => Grid!;

    public List<Button> Buttons { get; private set; } = new();

    int m_Rows;
    public int Rows
    {
        get => m_Rows;
        set
        {
            m_Rows = value;

            var RowDefinitions = Grid.RowDefinitions;
            RowDefinitions.Clear();

            for (int i = 0; i < m_Rows; i++)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
    }

    int m_Columns;
    public int Columns
    {
        get => m_Columns;
        set
        {
            m_Columns = value;

            var ColumnDefinitions = Grid.ColumnDefinitions;
            ColumnDefinitions.Clear();

            for (int i = 0; i < m_Columns; i++)
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    }

    public ButtonView(MainWindow window, int? rows = null, int? columns = null)
    {
        Window = window;
        Grid = new Grid();

        if (rows != null) Rows = rows.Value;
        if (columns != null) Columns = columns.Value;

        InitGrid();
    }

    public virtual void OnSetMainView() {}

    void InitGrid()
    {
        Grid.Margin = new(5);

        Style style = new Style();
        style.TargetType = typeof(Button);
        style.Setters.Add(new Setter(Button.FontSizeProperty, 16d));
        style.Setters.Add(new Setter(Button.MarginProperty, new Thickness(2)));
        Grid.Resources.Add(typeof(Button), style);
    }

    public void SetButton(Button button, int row, int column)
    {
        if (row >= Rows) Rows = row;
        if (column >= Columns) Columns = column;

        Buttons.Add(button);

        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        Grid.Children.Add(button);
    }
}
