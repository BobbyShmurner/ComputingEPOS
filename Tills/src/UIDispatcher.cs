using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingEPOS.Tills;

public class UIDispatcher : Singleton<UIDispatcher> {
    public MainWindow Window => MainWindow.Instance;

    Queue<Action> uiThreadActions = new();
    Queue<Action> uiActions = new();

    Thread asyncLoop = CreateAsyncLoop();

    public static void EnqueueUIAction(Func<Task> func) =>
        EnqueueUIAction(func());

    public static void EnqueueUIAction(Action action) {
        Instance.uiActions.Enqueue(action);
    }

    public static void EnqueueUIAction(Task task) =>
        Instance.uiActions.Enqueue(() => task.Wait());

    static Thread CreateAsyncLoop() {
        Thread thread = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            Action? action;

            while (true) {
                if (Instance.uiActions.TryDequeue(out action)) {
                    try {
                        action();
                    } catch (Exception ex) {
                        Trace.TraceError("Error in UI Async Loop: ");
                        Trace.TraceError(ex.Message + "\n");
                    }
                }
            }
        });

        thread.Start();
        return thread;
    }

    public static void DispatchOnUIThreadSingle(Action action) =>
        Instance.Window.Dispatcher.Invoke(action);

    public static void EnqueueUIUpdate(Action action) =>
        Instance.uiThreadActions.Enqueue(action);

    public static void EnqueueAndDispatchUIUpdate(Action action) {
        EnqueueUIUpdate(action);
        UpdateUI();
    }

    public static Task EnqueueAndDispatchUIUpdateAsync(Action action) {
        EnqueueUIUpdate(action);
        return UpdateUIAsync();
    }

    public static void UpdateUI() {
        Instance.Window.Dispatcher.Invoke(() => {
        while (Instance.uiThreadActions.Count > 0)
            Instance.uiThreadActions.Dequeue().Invoke();
    });
    }

    public async static Task UpdateUIAsync() =>
        await Instance.Window.Dispatcher.BeginInvoke(UpdateUI);
}
