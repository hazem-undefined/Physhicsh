using Physhicsh.Utils;

namespace Physhicsh.Extensions;

public static class AetherVectorsExtension
{
    public static Vector2Int ToVector2Int(this AethVec2 v) => new((int)v.X, (int)v.Y);
    public static AethVec2 ToAetherVector2(this SysVec2 v) => new(v.X, v.Y);

    public static AethVec2 WorldToScreen(this AethVec2 world) => new(
        Config.SystemConfig.ScreenCenter.X + (world.X * Config.SystemConfig.Instance.Graphics.PPM),
        Config.SystemConfig.ScreenCenter.Y - (world.Y * Config.SystemConfig.Instance.Graphics.PPM));

    public static AethVec2 ScreenToWorld(this AethVec2 screen) => new(
        (screen.X - Config.SystemConfig.ScreenCenter.X) / Config.SystemConfig.Instance.Graphics.PPM,
        (Config.SystemConfig.ScreenCenter.Y - screen.Y) / Config.SystemConfig.Instance.Graphics.PPM);

    public static AethVec3 ToVector3(this RayColor c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f);

    public static SysVec3 ToSysVector3(this RayColor c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f);

    public static ImColorA ToVector4(this RayColor c) =>
        new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

    public static RayColor ToColor(this AethVec3 c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)255);

    public static RayColor ToColor(this SysVec3 c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)255);

    public static RayColor ToColor(this ImColorA c) => new(
        (byte)(c.X * 255f),
        (byte)(c.Y * 255f),
        (byte)(c.Z * 255f),
        (byte)(c.W * 255f));

    public static AethVec3 ToAetherVector3(this SysVec3 v) =>
        new(v.X, v.Y, v.Z);

    public static SysVec3 ToNumericsVector3(this AethVec3 v) =>
        new(v.X, v.Y, v.Z);

    public static AethVec2 ToAethVec2(this SysVec2 v) => new(v.X, v.Y);
    public static SysVec2 ToSysVec2(this AethVec2 v) => new(v.X, v.Y);
}