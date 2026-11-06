using Tomlyn.Serialization;

namespace Physhicsh.Config;

public class LimitsConfig
{
    public float CircleSpawnRadiusMin { get; set; } = 0f;
    public float CircleSpawnRadiusMax { get; set; } = 3f;
    public float RectangleSpawnSizeMin { get; set; } = 0.001f;
    public float RectangleSpawnSizeMax { get; set; } = 50f;
    public float BoundsLimitX { get; set; } = 100f;
    public float BoundsLimitY { get; set; } = 100f;
    public int GravityScaleLimit { get; set; } = 10;

    [TomlIgnore]
    public SysVec2 CircleSpawnRadiusLimit
    {
        get => new(CircleSpawnRadiusMin, CircleSpawnRadiusMax);
        set { CircleSpawnRadiusMin = value.X; CircleSpawnRadiusMax = value.Y; }
    }

    [TomlIgnore]
    public SysVec2 RectangleSpawnSizeLimit
    {
        get => new(RectangleSpawnSizeMin, RectangleSpawnSizeMax);
        set { RectangleSpawnSizeMin = value.X; RectangleSpawnSizeMax = value.Y; }
    }
    [TomlIgnore]
    public SysVec2 BoundsLimit
    {
        get => new(BoundsLimitX, BoundsLimitY);
        set { BoundsLimitX = value.X; BoundsLimitY = value.Y; }
    }
}