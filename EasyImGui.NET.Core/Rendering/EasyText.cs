using EasyImGui.NET.Common.Converters;
using Hexa.NET.ImGui;

namespace EasyImGui.NET.Core;

public static partial class Ez {

    /// <summary>
    /// Display text
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="color">Text color to use</param>
    public static void Text<TColor>(string text, TColor? color = default) {
        if (color == null) {
            ImGui.Text(text);
            return;
        }
        var v4c = EasyColorConverter.Convert(color);
        ImGui.TextColored(v4c, text);
    }

    /// <summary>
    /// Display text with font
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="font">Font to use</param>
    /// <param name="color">Color to use</param>
    public static void Text<TColor>(string text, EasyFont font, TColor? color = default) {
        ImGui.PushFont(font.Ptr);
        Text(text, color);
        ImGui.PopFont();
    }

    /// <summary>
    /// Display text with font and color
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="fontName">Name of font to use</param>
    /// <param name="fontSize">Size of font to use</param>
    /// <param name="color">Color to use</param>
    public static void Text<TColor>(string text, string fontName, int fontSize, TColor? color = default) {
        var font = EasyFont.GetFont(fontName, fontSize);
        Text(text, font, color);
    }

    /// <summary>Shorthand for ImGui.PushFont(font.Ptr)</summary>
    public static void PushFont(EzFont font) {
        ImGui.PushFont(font.Ptr);
    }

    /// <summary>Extension Shorthand for ImGui.PushFont(font.Ptr)</summary>
    public static void Push(this EzFont font) => PushFont(font);

}

[Obsolete("Use Ez instead")]
public static partial class Easy {

    /// <summary>
    /// Display text
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="color">Text color to use</param>
    public static void Text<TColor>(string text, TColor? color = default) {
        if (color == null) {
            ImGui.Text(text);
            return;
        }
        var v4c = EasyColorConverter.Convert(color);
        ImGui.TextColored(v4c, text);
    }

    /// <summary>
    /// Display text with font
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="font">Font to use</param>
    /// <param name="color">Color to use</param>
    public static void Text<TColor>(string text, EasyFont font, TColor? color = default) {
        ImGui.PushFont(font.Ptr);
        Text(text, color);
        ImGui.PopFont();
    }

    /// <summary>
    /// Display text with font and color
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="fontName">Name of font to use</param>
    /// <param name="fontSize">Size of font to use</param>
    /// <param name="color">Color to use</param>
    public static void Text<TColor>(string text, string fontName, int fontSize, TColor? color = default) {
        var font = EasyFont.GetFont(fontName, fontSize);
        Text(text, font, color);
    }
}
