using Hexa.NET.ImGui;
using System.Reflection;
using System.Text;

namespace EasyImGui.NET.Common;

// TODO Error handling
public class EasyResourceLoader {

    public Assembly? EntryAssembly;
    public string? EntryAssemblyName;
    public string[]? EntryAssemblyResourceNames;
    public string? EntryAssemblyResourcePath;

    public string ResourcePath(string resourceName) => $"{EntryAssemblyResourcePath}.{resourceName}";

    public void InitMainAssembly() {
        EntryAssembly = Assembly.GetEntryAssembly();
        if (EntryAssembly == null) return;
        EntryAssemblyName = EntryAssembly.GetName().Name;
        if(EntryAssemblyName == null) return;
        EntryAssemblyResourceNames = EntryAssembly.GetManifestResourceNames();
        EntryAssemblyResourcePath = $"{EntryAssemblyName}.Resources";
    }

    public bool ResourceExists(string resourcePath) => EntryAssemblyResourceNames != null && EntryAssemblyResourceNames.Contains(resourcePath);

    public ReadOnlySpan<byte> GetResourceBytes(string resourcePath) {
        if (EntryAssembly == null) return null;
        using Stream? resourceStream = EntryAssembly.GetManifestResourceStream(resourcePath);
        if (resourceStream == null) return null;
        using MemoryStream memoryStream = new();
        resourceStream.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }

    public ReadOnlySpan<byte> LoadTexture(TextureDefAttribute textureDef) {
        if(textureDef == null || textureDef.ResourceName == null) return null;
        var fp = ResourcePath(textureDef.ResourceName);
        if (!ResourceExists(fp)) return null;
        return GetResourceBytes(fp);
    }

    public unsafe ImFontPtr[] LoadFonts(FontDefAttribute fontDef) {

        List<ImFontPtr> ptrs = [];
        if (fontDef == null || fontDef.ResourceName == null) return [.. ptrs];
        var fp = ResourcePath(fontDef.ResourceName);
        if (!ResourceExists(fp)) return [.. ptrs];

        var bytes = GetResourceBytes(fp);


        foreach(var size in fontDef.Sizes) {
            var fontPtr = LoadFont(bytes, "consolas", size);
            ptrs.Add(fontPtr);
        }

        return [.. ptrs];
        //var fontPtr = LoadFont(bytes, "consolas", 16);

        //return (fontPtr, 16);
    }

    public unsafe ImFontPtr LoadFont(ReadOnlySpan<byte> fontData, string name, int size) {
        ImFontPtr fontPtr = null;
        fixed (byte* ptr = fontData) {
            var cfg = ImGui.ImFontConfig() with {
                FontDataOwnedByAtlas = false
            };
            var nameData = new Span<byte>(Encoding.UTF8.GetBytes($"{name}, {size}px"));
            nameData.CopyTo(cfg.Name);
            fontPtr = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(ptr, fontData.Length, size, cfg);
            cfg.Destroy();
        }

        return fontPtr;
    }

}
