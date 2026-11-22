using Physhicsh.Core;
using Physhicsh.Debug;
using Physhicsh.Utils;
using System;

namespace Physhicsh;

public static class Program
{
    public static int Main(string[] args)
    {
        int errorCode = 0;
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Error(ex.ToString());
            try
            {
                string terminating = (e.IsTerminating ? "terminating" : "non-terminating");
                errorCode = ExternC.MessageBoxW(IntPtr.Zero,
                    ex.ToString(),
                    $"Unhandled Exception ({terminating})",
                    ExternC.MB_ICONERROR | ExternC.MB_OK | ExternC.MB_TOPMOST);
            }
            catch { }
        };

        MainWindow window = new();
        window.Start();

        return errorCode;
    }
}