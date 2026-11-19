using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Physhicsh.Utils;

// Vibe coded
public static class Display
{
    // Bind to EnumDisplaySettingsW explicitly, and match CharSet on the struct.
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool EnumDisplaySettingsW(
        string? deviceName,
        int modeNum,
        ref DEVMODE devMode);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;              // <-- must be set to Marshal.SizeOf<DEVMODE>()
        public ushort dmDriverExtra;
        public uint dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public uint dmDisplayOrientation;
        public uint dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public ushort dmLogPixels;
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;

        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }

    public static List<(int Width, int Height)> GetAvailableResolutions(string? deviceName = null)
    {
        List<(int, int)> resolutions = new List<(int, int)>();
        DEVMODE devMode = new DEVMODE();
        devMode.dmSize = (ushort)Marshal.SizeOf<DEVMODE>(); // critical!

        for (int modeIndex = 0;
             EnumDisplaySettingsW(deviceName, modeIndex, ref devMode);
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