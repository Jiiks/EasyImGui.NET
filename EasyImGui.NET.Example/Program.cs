using Hexa.NET.ImGui;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using EasyImGui.NET.Windowing.OpenTK;
using EasyImGui.NET.Common;
using System.Numerics;
using EasyImGui.NET.Demos;

namespace EasyImGui.NET.Example;
// Resources can just as easily be defined in a separate class
// by implementing the IEasyDefines interface
internal class Colors : IEasyDefines {
    private const string PurpleHex = "#CA64DE";

    [ColorDef(PurpleHex)]
    public Vector4 Purple;

    // Alpha override, lighten and darken can be used to easily make different shades of the same color.

    // Override alpha to be 50%
    [ColorDef(PurpleHex, AlphaOverride = .5f)]
    public Vector4 PurpleTranslucent;

    // Lighten color by 50%
    [ColorDef(PurpleHex, Lighten = .5f)]
    public Vector4 PurpleLighter;

    // Darken color by 50%
    [ColorDef(PurpleHex, Darken = .5f)]
    public Vector4 PurpleDarker;
}

internal class TestWindow(GameWindowSettings? s = null, NativeWindowSettings? n = null) : EasyWindowOpenTK(s, n) {

    // Resource instance
    private readonly Colors _colors = new();

    // Defining colors can be used as a single color or multiple colors.
    [ColorDef("#00FFFF")] // Hex
    public Vector4 Aqua;

    [ColorDef("#00FFFF55")]
    public Vector4 AquaTranslucent;

    // Other formats besides hex can be used
    [ColorDef(123, 104, 238, 255)] // Rgb or Rgba
    public Vector4 MediumSlateBlue;
    [ColorDef(.68f, .85f, .9f, .6f)] // Rgb or Rgba float
    public Vector4 SkyBlueTranslucent;

    // Multi def can only be used with hex due to attribute limitations.
    [ColorsDef(["#FF5733", "#33FF57"])]
    public Vector4 RedOrange, FreshGreen;

    [ColorsDef(["#3357FF", "#FFC733"])]
    public Vector4 BrightBlue, WarmYellow;

    // Define a texture that's in resx
    // Processed means the texture has already been processed and doesn't require any processing.
    // Eg it's already been ran through StbImage and saved as a raw binary.
    [TextureDef(ResourceName = "testimg", Width = 123, Height = 128, Processed = true)]
    public uint TestTexture;

    // Define a font that's in resx with sizes 16 and 32
    [FontDef("consolas", 16, 32, ResourceName = "consolas.ttf")]
    public ImFontPtr Consolas16, Consolas32;

    [FontDef("xmtrafficbold", 16, 32, ResourceName = "XM TrafficBd.ttf")]
    public ImFontPtr XmTrafficBd16, XmTrafficBd32;

    // Each ImGuiWindow definition is called every frame after settingup ImGui frame.
    // Setting title will AutoBegin unless AutoBegin is set to false.
    // AutoBegin calls ImGui.Begin() before and AutoEnd calls ImGui.End() after
    [ImGuiWindow(AutoBegin = true, AutoEnd = true, Id = "ColorDemoWindow", Title = "Color Demo")]
    public void ColorDemoWindow(FrameEventArgs e) {
        ImGui.TextColored(Aqua, "Aqua");
        ImGui.TextColored(AquaTranslucent, "AquaTranslucent");
        ImGui.TextColored(RedOrange, "RedOrange");
        ImGui.TextColored(FreshGreen, "FreshGreen");
        ImGui.TextColored(BrightBlue, "BrightBlue");
        ImGui.TextColored(WarmYellow, "WarmYellow");
        ImGui.TextColored(MediumSlateBlue, "MediumSlateBlue");
        ImGui.TextColored(SkyBlueTranslucent, "SkyBlueTranslucent");
    }

    // Additional windows can be defined just as easy.
    [ImGuiWindow(AutoBegin = true, AutoEnd = true, Id = "ImageDemoWindow", Title = "Image Demo")]
    public void ImageDemoWindow(FrameEventArgs e) {
        ImGui.PushFont(Consolas32);
        ImGui.TextColored(_colors.Purple, "This is a second window");
        ImGui.TextColored(_colors.PurpleTranslucent, "PurpleTranslucent");
        ImGui.PopFont();
        ImGui.PushFont(XmTrafficBd16);
        ImGui.TextColored(_colors.PurpleLighter, "PurpleLighter");
        ImGui.PopFont();
        ImGui.PushFont(XmTrafficBd32);
        ImGui.TextColored(_colors.PurpleDarker, "PurpleDarker");
        ImGui.PopFont();
        ImGui.Image(TestTexture, new Vector2(128, 128));
    }

    public static TestWindow Create() {
        var gws = GameWindowSettings.Default;
        gws.UpdateFrequency = 30;
        var nws = NativeWindowSettings.Default;
        nws.TransparentFramebuffer = true;
        return new TestWindow(gws, nws);
    }
}

internal class Program() {
    public static void Main() {
       // var tw = TestWindow.Create();
       // tw.Run();
       // tw.Dispose();
        EasyFileExplorerDemo.RunDemo();
    }
}
