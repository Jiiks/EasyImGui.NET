using System.Reflection;

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

}
