using EasyImGui.NET.Common;
using Hexa.NET.ImGui;
using System.Text;

namespace EasyImGui.NET.Core;

internal static class EasyFontLoader {
    internal static unsafe EasyFont LoadFromMemory(ReadOnlySpan<byte> data, string name, int size, nint glyphRanges = 0, bool merge = false) {
        var io = ImGui.GetIO();
        var cfg = ImGui.ImFontConfig() with {
            MergeMode = merge,
            OversampleH = 1,
            OversampleV = 1,
            FontData = ImGui.MemAlloc((uint)data.Length),
            FontDataSize = data.Length,
            GlyphRanges = glyphRanges != 0 ? (uint*)glyphRanges : io.Fonts.GetGlyphRangesDefault(),
            SizePixels = size,
            FontDataOwnedByAtlas = true
        };
        var nameData = new Span<byte>(Encoding.UTF8.GetBytes($"{name}, {size}px"));
        nameData.CopyTo(cfg.Name);
        data.CopyTo(new Span<byte>(cfg.FontData, data.Length));

        var font = io.Fonts.AddFont(cfg);

        io.Fonts.Build(); // build here because glyph ranges need to be allocated when calling Build or GetTexDataAsRGBA32
        cfg.Destroy();

        return new EasyFont() {
            Name = name,
            Size = size,
            Ptr = font
        };
    }

}

public class EzFont {

    //private readonly EzFont _defaultFont = new() { Name = "Default", Size = 1 };

    public Dictionary<(string, int), EzFont> _fonts = [];
    public IDictionary<(string, int), EzFont> Fonts => _fonts;
    //public EzFont GetFont(string name, int size) => _fonts[(name, size)];

    private EzFont? GetFontP(string name, int size = 0) {
        if (_fonts.Count <= 0) return null;

        if (!_fonts.ContainsKey((name, size))) return null;
        return _fonts[(name, size)];
    }

    //public EzFont GetFont(string name, int size = 0) {
    //    var font = GetFontP(name, size);
    //    return font ?? _defaultFont;
    //}

    public required string Name { get; set; }
    public required int Size { get; set; }
    public ImFontPtr Ptr { get; set; } = ImFontPtr.Null;
    public static void Define() { }

}

[Obsolete("Use EzFont instead")]
public class EasyFont {
    public string Name = null!;
    public float Size = 0;
    public ImFontPtr Ptr = ImFontPtr.Null;

    private static readonly Dictionary<string, EasyFont> _fonts = [];
    /// <summary>Loaded fonts</summary>
    public static Dictionary<string, EasyFont> Fonts => _fonts;

    private static EasyFont? GetFontP(string name) {
        if (_fonts.Count <= 0) {
            EasyDebugger.WriteLine("EasyFont", $"Attempted to use font {name} when no fonts are loaded.");
            return null;
        }
        if (_fonts.TryGetValue(name, out EasyFont? value)) return value;
        EasyDebugger.WriteLine("EasyFont", $"Attempted to use font {name} when it hasn't been loaded.");
        return null;
    }

    /// <summary>
    /// Get a loaded font
    /// </summary>
    /// <param name="name">Font name</param>
    /// <param name="size">Font size</param>
    /// <returns>Font with name and size</returns>
    public static EasyFont GetFont(string name, int size) {
        var font = GetFontP($"{name}.{size}");
        return font ?? new EasyFont();
    }
    public static ImFontPtr GetFontPtr(string name, int size) => GetFont(name, size).Ptr;

    /// <summary>
    /// Load a font from memory.
    /// </summary>
    /// <param name="bytes">Font bytes</param>
    /// <param name="name">Font name</param>
    /// <param name="size">Size to load</param>
    /// <returns>Loaded font</returns>
    public static EasyFont FromMemory(ReadOnlySpan<byte> bytes, string name, int size) {
        var font = EasyFontLoader.LoadFromMemory(bytes, name, size);
        _fonts.Add($"{name}.{size}", font);
        return font;
    }

    /// <summary>
    /// Load a font from memory with multiple sizes.
    /// </summary>
    /// <param name="bytes">Font bytes</param>
    /// <param name="name">Font name</param>
    /// <param name="sizes">Sizes to load</param>
    /// <returns>Collection of loaded fonts</returns>
    public static IEnumerable<EasyFont> FromMemory(ReadOnlySpan<byte> bytes, string name, params int[] sizes) {
        List<EasyFont> fonts = [];
        foreach (var size in sizes) {
            var font = EasyFontLoader.LoadFromMemory(bytes, name, size);
            _fonts.Add($"{name}.{size}", font);
            fonts.Add(font);
        }
        return fonts;
    }

    /// <summary>
    /// Load a font from file
    /// </summary>
    /// <param name="filePath">Path to font</param>
    /// <returns>Loaded font</returns>
    public static EasyFont FromFile(string filePath) {
        return default!;
    }

}
