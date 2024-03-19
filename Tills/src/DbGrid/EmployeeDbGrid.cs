using ComputingEPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputingEPOS.Tills;

public class EmployeeDbGrid : DbGrid<Employee> {
    public override string Title => "Employees";

    protected override Task<List<Employee>> CollectData() =>
        Api.Employees.GetEmployees();

    protected override Task<Employee> SaveChanges(Employee employee) =>
        Api.Employees.PutEmployee(employee);

    protected override List<List<IDbField>> CollectFields() {
        var leftFields = new List<IDbField>();
        var centerFields = new List<IDbField>();
        var rightFields = new List<IDbField>();

        leftFields.Add(new StringDbField<Employee>("First Names", nameof(Employee.FirstNames)));
        leftFields.Add(new StringDbField<Employee>("Surname", nameof(Employee.LastName)));

        rightFields.Add(new StringDbField<Employee>("Email", nameof(Employee.Email)));
        rightFields.Add(new StringDbField<Employee>("Mobile", nameof(Employee.Mobile)));
        rightFields.Add(new StringDbField<Employee>("Role", nameof(Employee.Role)));

        return new List<List<IDbField>> { leftFields, centerFields, rightFields };
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Employee.EmployeeID), width: new DataGridLength(50)),
            new DataGridColumnInfo("First Names", nameof(Employee.FirstNames)),
            new DataGridColumnInfo("Surname", nameof(Employee.LastName)),
            new DataGridColumnInfo("Role", nameof(Employee.Role)),
            new DataGridColumnInfo("Date Joined", nameof(Employee.DateJoined), format: "{0:dd/MM/yyyy}"),
            new DataGridColumnInfo("Email", nameof(Employee.Email)),
            new DataGridColumnInfo("Mobile", nameof(Employee.Mobile)),
        };
    }
}