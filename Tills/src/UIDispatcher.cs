using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills;

public class UIDispatcher : Singleton<UIDispatcher> {
    public MainWindow Window => MainWindow.Instance;

    Queue<Action> uiActions = new();

    public static void Enqueue(Action action) =>
        Instance.uiActions.Enqueue(action);

    public static void UpdateUI() {
        Instance.Window.Dispatcher.Invoke(() => {
        while (Instance.uiActions.Count > 0)
            Instance.uiActions.Dequeue().Invoke();
    });
    }

    public async static Task UpdateUIAsync() =>
        await Instance.Window.Dispatcher.BeginInvoke(UpdateUI);
}
