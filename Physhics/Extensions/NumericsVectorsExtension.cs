using Physhicsh.PConfig;
using Physhicsh.Utils;

namespace Physhicsh.Extensions;

public static class NumericsVectorsExtension
{
    public static Vector2Int ToVector2Int(this AethVec2 v) => new((int)v.X, (int)v.Y);
    public static SysVec2 ToNumericsVector2(this AethVec2 v) => new(v.X, v.Y);
    public static SysVec2 AddInt(this SysVec2 v, int x) => new(v.X + x, v.Y + x);
    public static SysVec2 AddFloat(this SysVec2 v, float x) => new(v.X + x, v.Y + x);
    public static AethVec2 WorldToScreen(this SysVec2 world) => new(
        Config.ScreenCenter.X + (world.X * Config.Instance.Screen.PPM),
        Config.ScreenCenter.Y - (world.Y * Config.Instance.Screen.PPM));

    public static AethVec2 ScreenToWorld(this SysVec2 screen) => new(
        (screen.X - Config.ScreenCenter.X) / Config.Instance.Screen.PPM,
        (Config.ScreenCenter.Y - screen.Y) / Config.Instance.Screen.PPM);
}
