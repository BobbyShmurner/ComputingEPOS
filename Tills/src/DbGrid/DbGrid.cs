using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public abstract class DbGrid<T> : IDbGrid where T : ICopyable<T> {
    public ObservableCollection<T>? Data { get; private set; }
    public List<DataGridColumnInfo>? ColumnInfo { get; private set; }
    public List<List<IDbField>>? Fields { get; private set; }
    public abstract string Title { get; }

    public Type Type => typeof(T);
    public int? SelectedIndex { get; private set; }

    protected abstract Task<T> SaveChanges(T data);
    protected abstract Task<List<T>> CollectData();
    protected abstract List<List<IDbField>> CollectFields();
    protected abstract List<DataGridColumnInfo> GetColumnInfo();

    public T? EditingData { get; private set; }

    public void HideGrid(DataGrid grid) {
        grid.SelectionChanged -= OnSelectionChanged;
    }

    public async Task SaveChanges() {
        if (Data == null || Fields == null || SelectedIndex == null || EditingData == null) return;

        object editingData = UIDispatcher.DispatchOnUIThread(() => {
            object data = EditingData.Copy();

            foreach (var field in Fields.SelectMany(x => x)) {
                field.UpdateData(ref data);
            }

            return data;
        });

        T newData = await SaveChanges((T)editingData);
        UIDispatcher.DispatchOnUIThread(() => {
            int index = SelectedIndex.Value;
            Data[index] = newData;
            SelectedIndex = index;
        });
    }

    public async Task ShowGrid(DataGrid grid, StackPanel leftPanel, StackPanel centerPanel, StackPanel rightPanel, bool resetSelection) {
        HideGrid(grid);

        var data = await CollectData();
        Data = new ObservableCollection<T>(data);

        ColumnInfo = GetColumnInfo();

        UIDispatcher.EnqueueOnUIThread(() => {
            grid.ItemsSource = Data;
            grid.Columns.Clear();
            grid.SelectionChanged += OnSelectionChanged;
            
            if (resetSelection) SelectedIndex = null;

            foreach (var columnInfo in ColumnInfo) {
                var dataColumn = new DataGridTextColumn {
                    Header = columnInfo.Header,
                    Binding = new Binding(columnInfo.Binding) { StringFormat = columnInfo.Format},
                    Width = columnInfo.Width,
                };

                grid.Columns.Add(dataColumn);
            }

            CreateFields(leftPanel, centerPanel, rightPanel);
            SelectElement(SelectedIndex);
        });
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        DataGrid dataGrid = (DataGrid)sender;

        int index = dataGrid.SelectedIndex;
        SelectElement(index == -1 ? null : index);
    }

    public void SelectElement(int? i) {
        SelectedIndex = i;
        if (i == null || Data == null || Data.Count < i) return;

        EditingData = Data[i.Value].Copy();
        UpdateFields();
    }

    void UpdateFields() {
        if (Fields == null) return;
        if (EditingData == null) return;

        foreach (var field in Fields.SelectMany(x => x)) {
            field.SetData(EditingData);
        }
    }

    void CreateFields(StackPanel leftPanel, StackPanel centerPanel, StackPanel rightPanel) {
        leftPanel.Children.Clear();
        centerPanel.Children.Clear();
        rightPanel.Children.Clear();

        Fields = CollectFields();

        for (int i = 0; i < Fields.Count; i++) {
            for (int j = 0; j < Fields[i].Count; j++) {
                var field = Fields[i][j];
                var element = field.Display();

                if (i == 0) {
                    leftPanel.Children.Add(element);
                } else if (i == 1) {
                    centerPanel.Children.Add(element);
                } else {
                    rightPanel.Children.Add(element);
                }
            }
        }
    }

}