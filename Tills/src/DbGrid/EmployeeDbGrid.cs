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

    protected override Task<Employee> SaveChanges(Employee employee, bool createNew) {
        if (!createNew)
            return Api.Employees.PutEmployee(employee);
        else
            return Api.Employees.Create(employee)!;
    }

    protected override Task Delete(Employee employee) =>
        Api.Employees.DeleteEmployee(employee);

    protected override void CollectFields(List<IDbField> leftFields, List<IDbField> centerFields, List<IDbField> rightFields) {
        leftFields.Add(new StringDbField<Employee>("First Names", nameof(Employee.FirstNames)));
        leftFields.Add(new StringDbField<Employee>("Surname", nameof(Employee.LastName)));
        leftFields.Add(new IntDbField<Employee>("Age", nameof(Employee.Age)));

        centerFields.Add(new StringDbField<Employee>("Email", nameof(Employee.Email)));
        centerFields.Add(new StringDbField<Employee>("Mobile", nameof(Employee.Mobile)));

        rightFields.Add(new DecimalDbField<Employee>("Wage", nameof(Employee.Wage)));
        rightFields.Add(new EnumDbField<Employee, Employee.Roles>("Role", nameof(Employee.Role)));
    }

    protected override List<DataGridColumnInfo> GetColumnInfo() {
        return new List<DataGridColumnInfo> {
            new DataGridColumnInfo("ID", nameof(Employee.EmployeeID), width: new DataGridLength(25)),
            new DataGridColumnInfo("First Names", nameof(Employee.FirstNames), width: new DataGridLength(1, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Surname", nameof(Employee.LastName), width: new DataGridLength(1, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Age", nameof(Employee.Age), width: new DataGridLength(35)),
            new DataGridColumnInfo("Wage", nameof(Employee.Wage), width: new DataGridLength(40)),
            new DataGridColumnInfo("Role", nameof(Employee.Role), width: new DataGridLength(80)),
            new DataGridColumnInfo("Email", nameof(Employee.Email), width: new DataGridLength(2, DataGridLengthUnitType.Star)),
            new DataGridColumnInfo("Mobile", nameof(Employee.Mobile), width: new DataGridLength(85)),
            new DataGridColumnInfo("Joined", nameof(Employee.DateJoined), format: "{0:dd/MM/yyyy}", width: new DataGridLength(70)),
        };
    }
}