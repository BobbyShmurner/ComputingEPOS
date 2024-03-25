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

using ComputingEPOS.Models;

namespace ComputingEPOS.Tills;

public class Menu {
    public string Name { get; private set; }

    public MenuButton[,] Items { get; private set; } = new MenuButton[0, 0];

    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public Menu(string name, MenuButton[,] items, int? rows = null, int? columns = null)
    {
        Name = name;
        Items = items;
        Rows = rows ?? items.GetLength(0);
        Columns = columns ?? items.GetLength(1);

        ResizeOrdersArray();
    }

    void ResizeOrdersArray()
    {
        MenuButton[,] newArray = new MenuButton[Rows, Columns];

        int minRows = Math.Min(Rows, Items.GetLength(0));
        int minCols = Math.Min(Columns, Items.GetLength(1));

        for (int i = 0; i < minRows; i++)
            for (int j = 0; j < minCols; j++)
                newArray[i, j] = Items[i, j];

        Items = newArray;
    }
}
