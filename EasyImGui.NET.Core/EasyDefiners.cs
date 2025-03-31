using EasyImGui.NET.Common.Converters;
using Hexa.NET.ImGui;
using OpenTK.Windowing.Common;
using System.Numerics;

namespace EasyImGui.NET.Core;

[Obsolete("delete")]
public interface IEzDefines {

    public void DefineFonts();
    public void DefineTextures();
    public void DefineColors();

}

[Obsolete("delete")]
public class EzDefines {

    private EzFont? _defaultFont = null;
    private readonly Dictionary<(string, int), EzFont> _fonts = [];

    private readonly Dictionary<string, Vector4> _colors = [];

    public EzFont DefaultFont => _defaultFont ??= new EzFont() { Name = "_Default", Size = 0, Ptr = ImGui.GetIO().FontDefault };

    public EzFont Fonts(string name, int size = 0) {
        if (_fonts.Count <= 0) return DefaultFont;

        if (!_fonts.ContainsKey((name, size))) return DefaultFont;
        return _fonts[(name, size)];
    }

    public Vector4 Colors(string colorName) => _colors[colorName];

    protected void DefineFont(ReadOnlySpan<byte> bytes, string name, params int[] sizes) {
        foreach (var size in sizes) {
            var ef = EasyFontLoader.LoadFromMemory(bytes, name, size);
            _fonts.Add((name, size), new EzFont() {
                Name = name,
                Size = size,
                Ptr = ef.Ptr
            });
        }
    }

    protected void DefineColor<TColor>(TColor color, string colorName) where TColor : notnull {
        _colors[colorName] = EasyColorConverter.Convert(color);
    }

    private readonly List<(Action<FrameEventArgs>, Action<FrameEventArgs>?)> _windows = [];
    public IList<(Action<FrameEventArgs>, Action<FrameEventArgs>?)> Actions => _windows;
    public void DefineWindow(Action<FrameEventArgs> body, Action<FrameEventArgs>? begin) {
        _windows.Add((body, begin));
    }

    public virtual void DefineFonts() { }
    public virtual void DefineTextures() { }
    public virtual void DefineColors() { }
}

[Obsolete("delete")]
public static partial class Ez {

    public static void DefineFont(ReadOnlySpan<byte> bytes, string name, params int[] sizes) { }
    public static void DefineFont(string filePath, string name, params int[] sizes) { }

    public static void DefineColor() { }

    public static void DefineTexture() { }

    public static void GenDefines(DirectoryInfo outDir) { }

    public static void LoadDefines(IEzDefines defines) { }

}
