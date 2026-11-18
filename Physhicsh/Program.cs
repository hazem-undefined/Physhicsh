using Physhicsh.Core;
using Physhicsh.Debug;
using System;

namespace Physhicsh;

public static class Program
{
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Error(ex.ToString());
        };

        MainWindow window = new();
        window.Start();
    }
}