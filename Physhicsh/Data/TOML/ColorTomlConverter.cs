// Vibe coded

using System;
using System.Collections.Generic;
using System.Globalization;
using Tomlyn.Serialization;

namespace Physhicsh.Data.TOML;

public sealed class ColorTomlConverter : TomlConverter<RayColor>
{
    private static readonly Dictionary<string, RayColor> Named = new(StringComparer.OrdinalIgnoreCase)
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

    public override RayColor Read(TomlReader reader)
    {
        return reader.TokenType switch
        {
            TomlTokenType.String => ParseString(reader, reader.GetString()),
            TomlTokenType.StartArray => ReadArray(reader),
            TomlTokenType.StartTable => ReadTable(reader),
            _ => throw reader.CreateException(
                                "RayColor must be a name (\"Red\"), a hex string (\"#RRGGBB\"), " +
                                "an array [r, g, b(, a)] or a table { r, g, b, a }."),
        };
    }

    public override void Write(TomlWriter writer, RayColor value)
    {
        foreach ((string? name, RayColor color) in Named)
        {
            if (color == value)
            {
                writer.WriteStringValue(name);
                return;
            }
        }

        writer.WriteStartArray();
        writer.WriteIntegerValue(value.R);
        writer.WriteIntegerValue(value.G);
        writer.WriteIntegerValue(value.B);
        if (value.A != 255)
        {
            writer.WriteIntegerValue(value.A);
        }
        writer.WriteEndArray();
    }

    private static RayColor ParseString(TomlReader reader, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw reader.CreateException("RayColor string cannot be empty.");
        }

        text = text.Trim();

        if (text[0] == '#')
        {
            return ParseHex(reader, text.AsSpan(1));
        }

        // Tolerate "sky_blue", "sky-blue", "sky blue"
        string normalized = text.Replace("_", "").Replace("-", "").Replace(" ", "");
        if (Named.TryGetValue(normalized, out RayColor color))
        {
            return color;
        }

        throw reader.CreateException(
            $"Unknown color name '{text}'. Valid names: {string.Join(", ", Named.Keys)}.");
    }

    private static RayColor ParseHex(TomlReader reader, ReadOnlySpan<char> hex)
    {
        if (hex.Length is not (6 or 8))
        {
            throw reader.CreateException("Hex color must be #RRGGBB or #RRGGBBAA.");
        }

        byte r = HexByte(reader, hex.Slice(0, 2));
        byte g = HexByte(reader, hex.Slice(2, 2));
        byte b = HexByte(reader, hex.Slice(4, 2));
        byte a = hex.Length == 8 ? HexByte(reader, hex.Slice(6, 2)) : (byte)255;
        return new RayColor(r, g, b, a);
    }

    private static byte HexByte(TomlReader reader, ReadOnlySpan<char> pair)
    {
        if (!byte.TryParse(pair, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte value))
        {
            throw reader.CreateException($"Invalid hex digits '{pair.ToString()}' in color.");
        }
        return value;
    }

    // ---- array: [r, g, b] or [r, g, b, a] ---------------------------------------

    private static RayColor ReadArray(TomlReader reader)
    {
        Span<byte> channels = stackalloc byte[4];
        int count = 0;

        while (reader.Read() && reader.TokenType != TomlTokenType.EndArray)
        {
            if (reader.TokenType != TomlTokenType.Integer)
            {
                throw reader.CreateException("RayColor array elements must be integers (0-255).");
            }
            if (count == 4)
            {
                throw reader.CreateException("RayColor array must have 3 (RGB) or 4 (RGBA) elements.");
            }
            channels[count++] = ToByte(reader, reader.GetInt64());
        }

        if (count is not (3 or 4))
        {
            throw reader.CreateException("RayColor array must have 3 (RGB) or 4 (RGBA) elements.");
        }

        return new RayColor(channels[0], channels[1], channels[2], count == 4 ? channels[3] : (byte)255);
    }

    // ---- table: { r = .., g = .., b = .., a = .. } ------------------------------

    private static RayColor ReadTable(TomlReader reader)
    {
        byte r = 0, g = 0, b = 0, a = 255;
        bool hasR = false, hasG = false, hasB = false;

        while (reader.Read() && reader.TokenType != TomlTokenType.EndTable)
        {
            if (reader.TokenType != TomlTokenType.PropertyName)
            {
                throw reader.CreateException("Unexpected token inside color table.");
            }

            string key = reader.PropertyName!;
            reader.Read(); // advance to the value

            if (reader.TokenType != TomlTokenType.Integer)
            {
                throw reader.CreateException($"RayColor channel '{key}' must be an integer (0-255).");
            }

            byte value = ToByte(reader, reader.GetInt64());

            switch (key.ToLowerInvariant())
            {
                case "r": r = value; hasR = true; break;
                case "g": g = value; hasG = true; break;
                case "b": b = value; hasB = true; break;
                case "a": a = value; break;
                default:
                    throw reader.CreateException($"Unknown color channel '{key}'. Expected r, g, b, a.");
            }
        }

        if (!(hasR && hasG && hasB))
        {
            throw reader.CreateException("RayColor table requires at least r, g and b.");
        }

        return new RayColor(r, g, b, a);
    }

    private static byte ToByte(TomlReader reader, long value)
    {
        if (value is < 0 or > 255)
        {
            throw reader.CreateException($"RayColor channel value {value} is out of range (0-255).");
        }
        return (byte)value;
    }
}