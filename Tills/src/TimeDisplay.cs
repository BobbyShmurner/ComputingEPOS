using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ComputingEPOS.Tills;

public class TimeDisplay : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    DispatcherTimer dispatcherTimer;

    public TimeDisplay() {
        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        dispatcherTimer.Interval = TimeSpan.FromSeconds(0.5);
        dispatcherTimer.Start();
    }

    DateTime m_CurrentTime = DateTime.Now;
    public DateTime CurrentTime
    {
        get => m_CurrentTime;
        set {
            m_CurrentTime = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
        }
    }

    void dispatcherTimer_Tick(object? sender, EventArgs e) {
        CurrentTime = DateTime.Now;
    }
}
