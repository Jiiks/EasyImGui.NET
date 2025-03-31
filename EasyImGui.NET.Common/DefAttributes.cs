using EasyImGui.NET.Common.Converters;
using System.Numerics;
using System.Reflection;

namespace EasyImGui.NET.Common;

public interface IEasyDefines { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ColorDefAttribute : Attribute {
    public ColorDefAttribute() { }
    public ColorDefAttribute(string hexValue) => HexValue = hexValue;
    public string? HexValue;
    public ColorDefAttribute(int r, int g, int b, int a = 255) => Rgba = [r,g,b,a];
    public int[]? Rgba;
    public int[]? Rgb;
    public ColorDefAttribute(float r, float g, float b, float a = 1) => RgbaF = [r, g, b, a];
    public float[]? RgbaF;
    public float[]? RgbF;

    public float AlphaOverride = -1, Lighten = -1, Darken = -1;
};

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ColorsDefAttribute(string[] hexValues) : Attribute {
    public string[]? HexValues = hexValues;
};

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TextureDefAttribute : Attribute {
    public string? ResourceName;
    public string? FileName;
    public bool Processed = false;
};

public static class EasySetters {
    public static void SetColorDefs(object? sender, FieldInfo[]? fields) {
        if (sender == null || fields == null || fields.Length <= 0) return;
        foreach(var field in fields) {
            var attribute = field.GetCustomAttribute<ColorDefAttribute>();
            if (attribute == null) continue;

            Vector4 col = Vector4.Zero;

            if (attribute.HexValue != null) {
                col = EasyColorConverter.HexToVector4(attribute.HexValue);
            } else if (attribute.Rgba != null) {
                col = EasyColorConverter.RgbaIToVector4(attribute.Rgba[0], attribute.Rgba[1], attribute.Rgba[2], attribute.Rgba[3]);
            } else if (attribute.Rgb != null) {
                col = EasyColorConverter.RgbaIToVector4(attribute.Rgb[0], attribute.Rgb[1], attribute.Rgb[2]);
            } else if (attribute.RgbaF != null) {
                col = EasyColorConverter.RgbaFToVector4(attribute.RgbaF[0], attribute.RgbaF[1], attribute.RgbaF[2], attribute.RgbaF[3]);
            } else if (attribute.RgbF != null) {
                col = EasyColorConverter.RgbaFToVector4(attribute.RgbF[0], attribute.RgbF[1], attribute.RgbF[2]);
            }

            if(attribute.Lighten != -1) {
                col = EasyColor.Lighten(col, attribute.Lighten);
            } else if(attribute.Darken != -1) {
                col = EasyColor.Darken(col, attribute.Lighten);
            }
            if (attribute.AlphaOverride != -1) {
                col = col.SetAlpha(attribute.AlphaOverride);
            }

            field.SetValue(sender, col);
        }
    }

    public static void SetColorsDefs(object? sender, FieldInfo[]? fields) {
        if (sender == null || fields == null || fields.Length <= 0) return;

        var attributeIndex = 0;
        foreach (var field in fields) {
            var attribute = field.GetCustomAttribute<ColorsDefAttribute>();
            if(attribute == null) continue;
            if (attribute.HexValues == null) {
                attributeIndex = 0;
                continue;
            }
            var defsSize = attribute.HexValues.Length;
            if (attributeIndex >= defsSize) attributeIndex = 0;
            field.SetValue(sender, EasyColorConverter.HexToVector4(attribute.HexValues[attributeIndex]));
            attributeIndex++;
        }
    }
}
