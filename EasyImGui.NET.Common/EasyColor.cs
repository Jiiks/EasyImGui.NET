using System.Numerics;

namespace EasyImGui.NET.Common;

// TODO there should be some color struct probably instead of using Vector4


public static class EasyColor {

    /// <summary>
    /// Mix two colors
    /// </summary>
    /// <param name="color1">First color</param>
    /// <param name="color2">Second color</param>
    /// <param name="ratio">Mix ratio</param>
    /// <returns>Mixed color</returns>
    public static Vector4 Mix(this Vector4 color1, Vector4 color2, float ratio) {
        ratio = Math.Clamp(ratio, 0f, 1f);
        return new(
            color1.X * (1 - ratio) + color2.X * ratio,
            color1.Y * (1 - ratio) + color2.Y * ratio,
            color1.Z * (1 - ratio) + color2.Z * ratio,
            color1.W * (1 - ratio) + color2.W * ratio 
        );
    }

    /// <summary>
    /// Darken color by amount
    /// </summary>
    /// <param name="color">Color to darken</param>
    /// <param name="amount">Amount to darken by</param>
    /// <returns>Darkened color</returns>
    public static Vector4 Darken(this Vector4 color, float amount) {
        amount = Math.Clamp(amount, 0f, 1f);
        return new(
            color.X * (1 - amount),
            color.Y * (1 - amount),
            color.Z * (1 - amount),
            color.W
        );
    }

    /// <summary>
    /// Lighten color by amount
    /// </summary>
    /// <param name="color">Color to lighten</param>
    /// <param name="amount">Amount to lighten by</param>
    /// <returns>Lightened color</returns>
    public static Vector4 Lighten(this Vector4 color, float amount) {
        amount = Math.Clamp(amount, 0f, 1f);
        return new(
            Math.Clamp(color.X + amount, 0f, 1f),
            Math.Clamp(color.Y + amount, 0f, 1f),
            Math.Clamp(color.Z + amount, 0f, 1f),
            color.W
        );
    }

    public static Vector4 SetAlpha(this Vector4 color, float alpha) => color with { Z = alpha };

}