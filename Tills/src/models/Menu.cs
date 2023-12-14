using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComputingEPOS.Tills;

public class Menu {
    public struct MenuItem
    {
        public OrderListItem item;
        public string text;

        public MenuItem(OrderListItem item, string text)
        {
            this.item = item;
            this.text = text;
        }
    }

    public string Name { get; private set; }

    public MenuItem?[,] Items { get; private set; } = new MenuItem?[0, 0];

    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public Menu(string name, MenuItem?[,] items)
    {
        Name = name;
        Items = items;
        Rows = items.GetLength(0);
        Columns = items.GetLength(1);

        ResizeOrdersArray();
    }

    void ResizeOrdersArray()
    {
        MenuItem?[,] newArray = new MenuItem?[Rows, Columns];

        int minRows = Math.Min(Rows, Items.GetLength(0));
        int minCols = Math.Min(Columns, Items.GetLength(1));

        for (int i = 0; i < minRows; i++)
            for (int j = 0; j < minCols; j++)
                newArray[i, j] = Items[i, j];

        Items = newArray;
    }
}
