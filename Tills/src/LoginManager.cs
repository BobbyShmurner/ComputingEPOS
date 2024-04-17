using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ComputingEPOS.Common.Models;

namespace ComputingEPOS.Tills;

public class LoginManager : Singleton<LoginManager>, INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

	Employee? m_CurrentEmployee = null;
    public Employee? CurrentEmployee {
        get => m_CurrentEmployee;
        private set {
            m_CurrentEmployee = value;

            UIDispatcher.EnqueueOnUIThread(() => {
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentEmployee)));

            	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccessLevel)));
            	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmployeeName)));
            	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmployeeFirstName)));
            });
        }
    }

	public int AccessLevel {
		get {
			if (m_CurrentEmployee == null) return 0;

			switch (m_CurrentEmployee.Role) {
				case Employee.Roles.Cashier:
					return 0;
				case Employee.Roles.Kitchen:
					return 0;
				case Employee.Roles.Supervisor:
					return 1;
				case Employee.Roles.Manager:
					return 2;
			}

			return 0;
		}
	}

	public string? EmployeeName => CurrentEmployee != null ? CurrentEmployee.FirstNames + " " + CurrentEmployee.LastName : null;
	public string? EmployeeFirstName => CurrentEmployee?.FirstNames?.Split(' ')?[0];

	public async Task Login(string pin) {
		UIDispatcher.EnqueueAndUpdateOnUIThread(() => Modal.Instance.Show("Logging in..."));
		Employee? employee = await Api.Employees.GetEmployeeFromPin(pin);

		if (employee == null) {
			UIDispatcher.EnqueueOnUIThread(() => Modal.Instance.Show("Invalid pin!"));
			return;
		}

		CurrentEmployee = employee;
		await SwitchToMenuScreen();
	}

	async Task SwitchToMenuScreen() {
		OrderManager orderManager = UIDispatcher.DispatchOnUIThread(() => MainWindow.Instance.MenuView.OrderManager);

		Action? switchToMenuView = null;
		switchToMenuView = () => {
			MainWindow.Instance.MenuView.OrderMenuManager.ShowFirstMenu();
			MainWindow.Instance.MenuView.OrderMenuManager.OnMenusLoaded -= switchToMenuView!;

			Modal.Instance.Hide();
			MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.MenuView);
		};

		await orderManager.NextOrder();

		UIDispatcher.EnqueueOnUIThread(() => {
			MainWindow.Instance.MenuView.OrderMenuManager.OnMenusLoaded += switchToMenuView;

			if (MainWindow.Instance.MenuView.OrderMenuManager.LoadingMenus) {
				Modal.Instance.Show("Loading...", false);
			} else {
				switchToMenuView();    
			}
		});
	}

	public async Task Logout() {
		MenuView menuView = UIDispatcher.DispatchOnUIThread(() => MainWindow.Instance.MenuView);
		if (menuView.OrderManager.CurrentOrder != null) await menuView.OrderManager.DeleteCurrentOrder();
		await menuView.OrderManager.CloseAllPaidChecks();

		CurrentEmployee = null;
		UIDispatcher.EnqueueOnUIThread(() => MainWindow.Instance.RootViewManager.ShowView(MainWindow.Instance.LoginScreen));
	}
}