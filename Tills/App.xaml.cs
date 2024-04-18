using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommandLine;

namespace ComputingEPOS.Tills;

public class CLOptions {
    [Option('t', "till", Required = false, HelpText = "The till number to use.", Default = 1)]
    public int TillNumber { get; set; }

    [Option('f', "fullscreen", Required = false, HelpText = "Whether to run the application in fullscreen mode.", Default = false)]
    public bool FullScreen { get; set; }
}

public partial class App : Application {
    #pragma warning disable CS8618
    public static CLOptions Options { get; private set; }
    #pragma warning restore CS8618

    void App_Startup(object sender, StartupEventArgs e) {
        Options = Parser.Default.ParseArguments<CLOptions>(e.Args).Value;

        if (Options.TillNumber < 1) {
            MessageBox.Show("Invalid till number specified. Please specify a valid till number.", "Invalid Till Number", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        MainWindow window = new MainWindow();

        if (Options.FullScreen) {
            window.WindowState = WindowState.Maximized;
            window.WindowStyle = WindowStyle.None;
        }

        window.Show();
    }
}
