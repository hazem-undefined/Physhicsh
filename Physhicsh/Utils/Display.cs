using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Physhicsh.Utils;

// Vibe coded
public static class Display
{
    public static List<(int Width, int Height)> GetAvailableResolutions(string? deviceName = null)
    {
        List<(int, int)> resolutions = new List<(int, int)>();
        ExternC.DEVMODE devMode = new ExternC.DEVMODE
        {
            dmSize = (ushort)Marshal.SizeOf<ExternC.DEVMODE>() // critical!
        };

        for (int modeIndex = 0;
             ExternC.EnumDisplaySettingsW(deviceName, modeIndex, ref devMode);
             modeIndex++)
        {
            resolutions.Add(((int)devMode.dmPelsWidth, (int)devMode.dmPelsHeight));
        }

        return resolutions.Distinct().ToList();
    }

    public static string[] GetAvailableResolutionsStrings(string? deviceName = null)
    {
        List<(int Width, int Height)> available = GetAvailableResolutions();
        List<string> inStrings = [];
        foreach ((int Width, int Height) item in available.ToArray())
        {
            inStrings.Add(item.ToString());
        }
        return inStrings.ToArray();
    }
}