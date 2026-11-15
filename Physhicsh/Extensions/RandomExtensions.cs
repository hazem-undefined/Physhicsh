using System;
using System.Numerics;

namespace Physhicsh.Extensions;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, float min, float max)
    {
        if (random is null) throw new ArgumentNullException(nameof(random));
        if (min > max) throw new ArgumentException("min must be <= max");
        return min + (float)random.NextDouble() * (max - min);
    }

    private static double _hue;

    public static RayColor RandomColor(this Random rng)
    {
        _hue = (_hue + 137.508) % 360.0;

        double hue = _hue;
        double saturation = 0.70 + rng.NextDouble() * 0.30;
        double value = 0.80 + rng.NextDouble() * 0.20;

        return FromHsv(hue, saturation, value);
    }

    private static RayColor FromHsv(double h, double s, double v)
    {
        double c = v * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = v - c;

        (double r, double g, double b) = h switch
        {
            < 60 => (c, x, 0d),
            < 120 => (x, c, 0d),
            < 180 => (0d, c, x),
            < 240 => (0d, x, c),
            < 300 => (x, 0d, c),
            _ => (c, 0d, x),
        };
        return new RayColor(
            (byte)Math.Round((r + m) * 255),
            (byte)Math.Round((g + m) * 255),
            (byte)Math.Round((b + m) * 255));
    }

    public static Vector4 RandomImGuiColor(this Random rng)
    {
        return new(
        rng.NextFloat(0, 1f),
        rng.NextFloat(0, 1f),
        rng.NextFloat(0, 1f),
        255f);
    }
}
