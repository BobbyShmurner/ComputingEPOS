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

    public static void DispatchSingle(Action action) =>
        Instance.Window.Dispatcher.Invoke(action);

    public static void Enqueue(Action action) =>
        Instance.uiActions.Enqueue(action);

    public static void EnqueueAndDispatch(Action action) {
        Enqueue(action);
        UpdateUI();
    }

    public static Task EnqueueAndDispatchAsync(Action action) {
        Enqueue(action);
        return UpdateUIAsync();
    }

    public static void UpdateUI() {
        Instance.Window.Dispatcher.Invoke(() => {
        while (Instance.uiActions.Count > 0)
            Instance.uiActions.Dequeue().Invoke();
    });
    }

    public async static Task UpdateUIAsync() =>
        await Instance.Window.Dispatcher.BeginInvoke(UpdateUI);
}
