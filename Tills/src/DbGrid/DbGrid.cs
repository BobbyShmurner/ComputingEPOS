using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ComputingEPOS.Tills;

public abstract class DbGrid<T> : IDbGrid where T : class, ICopyable<T>, new() {
    public ObservableCollection<T>? Data { get; private set; }
    public List<DataGridColumnInfo>? ColumnInfo { get; private set; }
    public List<List<IDbField>>? Fields { get; private set; }
    public abstract string Title { get; }

    public Type Type => typeof(T);
    public int? SelectedIndex { get; private set; }

    protected abstract Task Delete(T data);
    protected abstract Task<List<T>> CollectData();
    protected abstract List<List<IDbField>> CollectFields();
    protected abstract List<DataGridColumnInfo> GetColumnInfo();
    protected abstract Task<T> SaveChanges(T data, bool createNew);

    public T? EditingData { get; private set; }

    DataGrid? Grid;
    DataGrid? AddGrid;

    public void HideGrid() {
        if (Grid != null) Grid.SelectionChanged -= OnSelectionChanged;
        if (AddGrid != null) AddGrid.SelectionChanged -= OnAddGridSelectionChanged;
    }

    public async Task SaveChanges() {
        if (Fields == null || Data == null) return;

        object editingData = UIDispatcher.DispatchOnUIThread(() => {
            object data = EditingData?.Copy() ?? new T{ };

            foreach (var field in Fields.SelectMany(x => x)) {
                field.UpdateData(ref data);
            }

            return data;
        });

        T newData = await SaveChanges((T)editingData, SelectedIndex == null);

        UIDispatcher.DispatchOnUIThread(() => {
            if (SelectedIndex != null) {
                int index = SelectedIndex.Value;
                Data[SelectedIndex.Value] = newData;
                SelectedIndex = index;
            } else { 
                Data.Add(newData);
                SelectedIndex = Data.Count - 1;
            }

            if (Grid != null) Grid.SelectedIndex = SelectedIndex.Value;
            if (AddGrid != null) AddGrid.SelectedIndex = SelectedIndex.Value;
        });
    }

    public async Task DeleteCurrent() {
        if (Data == null) return;
        if (SelectedIndex == null) return;

        await Delete(Data[SelectedIndex.Value]);

        SelectedIndex = null;
    }

    public async Task ShowGrid(DataGrid grid, DataGrid addGrid, StackPanel leftPanel, StackPanel centerPanel, StackPanel rightPanel, bool resetSelection) {
        HideGrid();
        Grid = grid;
        AddGrid = addGrid;

        if (Grid == null || AddGrid == null) return;

        var data = await CollectData();
        Data = new ObservableCollection<T>(data);

        ColumnInfo = GetColumnInfo();

        UIDispatcher.EnqueueOnUIThread(() => {
            AddGrid.SelectionChanged += OnAddGridSelectionChanged;

            var addSource = new string[1][];
            addSource[0] = new string[ColumnInfo.Count];
            AddGrid.ItemsSource = addSource;
            AddGrid.Columns.Clear();

            Grid.ItemsSource = Data;
            Grid.Columns.Clear();
            Grid.SelectionChanged += OnSelectionChanged;
            
            if (resetSelection) SelectedIndex = null;

            for (int i = 0; i < ColumnInfo.Count; i++) {
                var columnInfo = ColumnInfo[i];

                var dataColumn = new DataGridTextColumn {
                    Header = columnInfo.Header,
                    Binding = new Binding(columnInfo.Binding) { StringFormat = columnInfo.Format},
                    Width = columnInfo.Width,
                };

                Grid.Columns.Add(dataColumn);

                var addColumn = new DataGridTextColumn {
                    Width = columnInfo.Width,
                    Binding = new Binding($"[{i}]"),
                };

                AddGrid.Columns.Add(addColumn);
            }

            CreateFields(leftPanel, centerPanel, rightPanel);
            SelectElement(SelectedIndex);
        });
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        DataGrid dataGrid = (DataGrid)sender;

        int index = dataGrid.SelectedIndex;
        if (index == -1) return;

        SelectElement(index);
    }

    void OnAddGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
        DataGrid addGrid = (DataGrid)sender;

        int index = addGrid.SelectedIndex;
        if (index == -1) return;

        SelectElement(null);
    }

    public void SelectElement(int? i) {
        SelectedIndex = i;
        if (SelectedIndex == null || Data == null || i == null) {
            EditingData = null;
            if (Grid != null) Grid.SelectedIndex = -1;
        } else {
            EditingData = Data[i.Value].Copy();
            if (AddGrid != null) AddGrid.SelectedIndex = -1;
        }

        UpdateFields();
    }

    void UpdateFields() {
        if (Fields == null) return;

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