using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ComputingEPOS.Tills;

public class UIDispatcher : Singleton<UIDispatcher> {
    public static MainWindow Window => MainWindow.Instance;

    Queue<Action> uiThreadActions = new();
    Queue<Action> uiActions = new();

    Thread asyncLoop = CreateAsyncLoop();
    Dispatcher dispatcher = Window.Dispatcher;

    /// <summary>
    /// Enqueue an async action triggered by UI to be executed.
    /// </summary>
    /// <param name="func">The async action to execute.</param>
    public static void EnqueueUIAction(Func<Task> func) =>
        EnqueueUIAction(() => func().Wait());

    /// <summary>
    /// Enqueue an action triggered by UI to be executed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void EnqueueUIAction(Action action) =>
        Instance.uiActions.Enqueue(action);

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

    /// <summary>
    /// Immediatly dispatch an action on the UI thread. This does not trigger a UI Update.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void DispatchOnUIThread(Action action) =>
        Instance.dispatcher.Invoke(action);

    /// <summary>
    /// Adds an action to a queue of actions to be dispatched on the UI thread. <br/>
    /// To dispatch the actions, see <see cref="UpdateUI"/>.
    /// </summary>
    /// <param name="action"></param>
    public static void EnqueueOnUIThread(Action action) =>
        Instance.uiThreadActions.Enqueue(action);

    /// <summary>
    /// Adds an action to the end of the UI queue, and then dispatches all events in the queue. <br/>
    /// Equivalent to calling <see cref="EnqueueOnUIThread"/> followed by <see cref="UpdateUI"/>.
    /// </summary>
    /// <param name="action"></param>
    public static void EnqueueAndUpdateOnUIThread(Action action) {
        EnqueueOnUIThread(action);
        UpdateUI();
    }

    /// <summary>
    /// Dispatches all queued UI actions on the UI thread.
    /// </summary>
    public static void UpdateUI() {
        Instance.dispatcher.Invoke(() => {
            while (Instance.uiThreadActions.Count > 0)
                Instance.uiThreadActions.Dequeue().Invoke();
        });
    }
}
