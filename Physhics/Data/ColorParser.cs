using System;
using System.Collections.Generic;
using System.Globalization;
using Tomlyn.Model;

namespace Physhicsh.Data;

public static class ColorParser
{
    private static readonly Dictionary<string, RayColor> NamedRayColor =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["LightGray"] = RayColor.LightGray,
            ["Gray"] = RayColor.Gray,
            ["DarkGray"] = RayColor.DarkGray,
            ["Yellow"] = RayColor.Yellow,
            ["Gold"] = RayColor.Gold,
            ["Orange"] = RayColor.Orange,
            ["Pink"] = RayColor.Pink,
            ["Red"] = RayColor.Red,
            ["Maroon"] = RayColor.Maroon,
            ["Green"] = RayColor.Green,
            ["Lime"] = RayColor.Lime,
            ["DarkGreen"] = RayColor.DarkGreen,
            ["SkyBlue"] = RayColor.SkyBlue,
            ["Blue"] = RayColor.Blue,
            ["DarkBlue"] = RayColor.DarkBlue,
            ["Purple"] = RayColor.Purple,
            ["Violet"] = RayColor.Violet,
            ["DarkPurple"] = RayColor.DarkPurple,
            ["Beige"] = RayColor.Beige,
            ["Brown"] = RayColor.Brown,
            ["DarkBrown"] = RayColor.DarkBrown,
            ["White"] = RayColor.White,
            ["Black"] = RayColor.Black,
            ["Blank"] = RayColor.Blank,
            ["Magenta"] = RayColor.Magenta,
            ["RayWhite"] = RayColor.RayWhite,
        };

    public static RayColor Parse(object? value) => value switch
    {
        null => throw new FormatException("Color is missing"),
        string s => ParseString(s),
        TomlArray a => ParseArray(a),
        _ => throw new FormatException(
                          $"Unsupported color type: {value.GetType()}"),
    };

    private static RayColor ParseString(string s)
    {
        s = s.Trim();

        // 1) Named color
        if (NamedRayColor.TryGetValue(s, out RayColor named))
            return named;

        // 2) "r, g, b" or "r, g, b, a"
        string[] parts = s.Split(',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 3 or 4)
        {
            return new RayColor(
                byte.Parse(parts[0], CultureInfo.InvariantCulture),
                byte.Parse(parts[1], CultureInfo.InvariantCulture),
                byte.Parse(parts[2], CultureInfo.InvariantCulture),
                parts.Length == 4
                    ? byte.Parse(parts[3], CultureInfo.InvariantCulture)
                    : (byte)255);
        }

        throw new FormatException($"Invalid color string: \"{s}\"");
    }

    private static RayColor ParseArray(TomlArray arr)
    {
        if (arr.Count is not (3 or 4))
            throw new FormatException(
                $"Color array must have 3 or 4 elements, got {arr.Count}");

        return new RayColor(
            Convert.ToByte(arr[0], CultureInfo.InvariantCulture),
            Convert.ToByte(arr[1], CultureInfo.InvariantCulture),
            Convert.ToByte(arr[2], CultureInfo.InvariantCulture),
            arr.Count == 4
                ? Convert.ToByte(arr[3], CultureInfo.InvariantCulture)
                : (byte)255);
    }
}