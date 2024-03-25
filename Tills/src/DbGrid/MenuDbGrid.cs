using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class MenuDbGrid : DbGrid<Models.Menu> {
    public override string Title => "Menus";

    protected override Task<List<Models.Menu>> CollectData() =>
        Api.Menus.GetMenus();

    protected override Task<Models.Menu> SaveChanges(Models.Menu menu, bool createNew) {
        if (!createNew)
            return Api.Menus.PutMenu(menu);
        else
            return Api.Menus.Create(menu)!;
    }

    protected override Task Delete(Models.Menu menu) =>
        Api.Menus.DeleteMenu(menu);

    protected override void CollectFields(List<IDbField> leftFields, List<IDbField> centerFields, List<IDbField> rightFields) {
        leftFields.Add(new StringDbField<Models.Menu>("Name", nameof(Models.Menu.Name)));
        rightFields.Add(new IntNullDbField<Models.Menu>("Rows", nameof(Models.Menu.Rows)));
        rightFields.Add(new IntNullDbField<Models.Menu>("Columns", nameof(Models.Menu.Columns)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Models.Menu.MenuID), width: new DataGridLength(50)),
            new DataGridColumnInfo("Name", nameof(Models.Menu.Name)),
            new DataGridColumnInfo("Date Created", nameof(Models.Menu.Date), format: "{0:dd/MM/yyyy}"),
            new DataGridColumnInfo("Rows", nameof(Models.Menu.Rows)),
            new DataGridColumnInfo("Columns", nameof(Models.Menu.Columns)),
        };
    }
}