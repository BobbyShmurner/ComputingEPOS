﻿using System;
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

    public static void EnqueueUIAction(Func<Task> func) =>
        EnqueueUIAction(() => func().Wait());

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

    public static void DispatchOnUIThreadSingle(Action action) =>
        Instance.dispatcher.Invoke(action);

    public static void EnqueueUIUpdate(Action action) =>
        Instance.uiThreadActions.Enqueue(action);

    public static void EnqueueAndDispatchUIUpdate(Action action) {
        EnqueueUIUpdate(action);
        UpdateUI();
    }

    public static void UpdateUI() {
        Instance.dispatcher.Invoke(() => {
            while (Instance.uiThreadActions.Count > 0)
                Instance.uiThreadActions.Dequeue().Invoke();
        });
    }
}
