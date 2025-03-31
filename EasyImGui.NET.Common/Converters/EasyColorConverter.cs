using OpenTK.Mathematics; // TODO remove references to OpenTK in common
using System.Drawing;

using Vector4 = System.Numerics.Vector4;

namespace EasyImGui.NET.Common.Converters;

/// <summary>
/// Converts different types of colors to <see cref="System.Numerics.Vector4"/>
/// </summary>
public static class EasyColorConverter {
    private static readonly Dictionary<object, Vector4> _colorCache = [];
    public static int GetCacheSize => _colorCache.Count;
    public static bool NoCache { get; set; } = true; // The cache is somewhat useless maybe.
    private static Vector4 Cache(object what, Vector4 color) {
        if(!NoCache) _colorCache[what] = color;
        return color;
    }

    /// <summary>
    /// Convert <see cref="System.Drawing.Color"/> to <see cref="System.Numerics.Vector4"/>
    /// </summary>
    /// <param name="color">Color to convert</param>
    public static Vector4 ToVector4(this Color color) {
        if (!NoCache && _colorCache.TryGetValue(color, out Vector4 value)) return value;
        return Cache(color, new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
    }
    // <summary>
    /// Convert <see cref="OpenTK.Mathematics.Color4"/> to <see cref="System.Numerics.Vector4"/>
    /// </summary>
    /// <param name="color">Color to convert</param>
    [Obsolete("There should not be references to OpenTK in common")]
    public static Vector4 ToVector4(this Color4 color) {
        if (!NoCache && _colorCache.TryGetValue(color, out Vector4 value)) return value;
        return Cache(color, new(color.R, color.G, color.B, color.A));
    }

    /// <summary>
    /// Convert rgba byte values to <see cref="System.Numerics.Vector4"/>
    /// </summary>
    public static Vector4 RgbaBToVector4(byte r, byte g, byte b, byte a = 255) {
        if (!NoCache && _colorCache.TryGetValue((r,g,b,a), out Vector4 value)) return value;
        return Cache((r, g, b, a), new(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
    }

    /// <summary>
    /// Convert rgba int values to <see cref="System.Numerics.Vector4"/>
    /// </summary>
    public static Vector4 RgbaIToVector4(int r, int g, int b, int a = 255) {
        if (!NoCache && _colorCache.TryGetValue((r, g, b, a), out Vector4 value)) return value;
        return Cache((r, g, b, a), new(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
    }
    /// <summary>
    /// Convert rgba float values to <see cref="System.Numerics.Vector4"/>
    /// </summary>
    public static Vector4 RgbaFToVector4(float r, float g, float b, float a = 1) {
        if (!NoCache && _colorCache.TryGetValue((r, g, b, a), out Vector4 value)) return value;
        return Cache((r, g, b, a), new(r, g, b, a));
    }

    /// <summary>
    /// Convert HEX string to <see cref="System.Numerics.Vector4"/>. Supports both 4 and 8 byte HEX values.
    /// </summary>
    /// <param name="hex">Hex color to convert</param>
    public static Vector4 HexToVector4(string hex) {
        hex = hex.TrimStart('#');
        if (!NoCache && _colorCache.TryGetValue(hex, out Vector4 value)) return value;

        byte r, g, b, a = 255;
        if (hex.Length == 6) {          // RRGGBB
            r = System.Convert.ToByte(hex[..2], 16);
            g = System.Convert.ToByte(hex[2..4], 16);
            b = System.Convert.ToByte(hex[4..6], 16);
        } else if (hex.Length == 8) {   // RRGGBBAA
            r = System.Convert.ToByte(hex[..2], 16);
            g = System.Convert.ToByte(hex[2..4], 16);
            b = System.Convert.ToByte(hex[4..6], 16);
            a = System.Convert.ToByte(hex[6..8], 16);
        } else {
            throw new ArgumentException($"Invalid hex string format. Must be RRGGBB or RRGGBBAA. Was {hex}");
        }
        return RgbaBToVector4(r, g, b, a);
    }

    /// <summary>
    /// Convert HEX string to <see cref="System.Numerics.Vector4"/> using bitwise ops. Supports both 4 and 8 byte HEX values.
    /// </summary>
    /// <param name="hex">Hex color to convert</param>
    public static Vector4 HexToVector4BitShift(string hex) {
        hex = hex.TrimStart('#');

        if (!NoCache && _colorCache.TryGetValue(hex, out Vector4 cachedValue))
            return cachedValue;

        uint colorValue;
        byte r, g, b, a = 255;
        if (hex.Length == 6) {          // RRGGBB
            colorValue = System.Convert.ToUInt32(hex, 16);
            r = (byte)((colorValue >> 16) & 0xFF);
            g = (byte)((colorValue >> 8) & 0xFF);
            b = (byte)(colorValue & 0xFF);
        } else if (hex.Length == 8) {   // RRGGBBAA
            colorValue = System.Convert.ToUInt32(hex, 16);
            r = (byte)((colorValue >> 24) & 0xFF);
            g = (byte)((colorValue >> 16) & 0xFF);
            b = (byte)((colorValue >> 8) & 0xFF);
            a = (byte)(colorValue & 0xFF);
        } else {
            throw new ArgumentException($"Invalid hex string format. Must be RRGGBB or RRGGBBAA. Was {hex}");
        }
        return RgbaBToVector4(r, g, b, a);
    }

    /// <summary>
    /// Convert supported color types to <see cref="Vector4"/>
    /// </summary>
    /// <param name="color">Color object to convert</param>
    /// <returns>Converted color</returns>
    public static Vector4 Convert(object color) {
        return color switch {
            Vector4 => (Vector4)color,
            Color4 c => c.ToVector4(),
            Color c => c.ToVector4(),
            string hex => HexToVector4BitShift(hex),
            (float r, float g, float b, float a) => RgbaFToVector4(r, g, b, a),
            (byte r, byte g, byte b, byte a) => RgbaBToVector4(r, g, b, a),
            (int r, int g, int b, int a) => RgbaIToVector4(r, g, b, a),
            _ => throw new ArgumentException($"Unsupported color type. {color.GetType()}")
        };
    }

    /// <summary>
    /// Pack a Vector4 color into uint
    /// </summary>
    /// <param name="color">Color to pack</param>
    /// <returns>Packed color</returns>
    public static uint Pack(Vector4 color) =>
        ((uint)(color.W* 255) << 24) |
        ((uint)(color.X* 255) << 16) |
        ((uint)(color.Y* 255) << 8)  |
        (uint) (color.Z* 255);


}
