using EasyImGui.NET.Common;
using EasyImGui.NET.Components;
using EasyImGui.NET.Windowing.OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.ComponentModel;

namespace EasyImGui.NET.Demos;

/// <summary>
/// This is currently just testing different ways of doing components.
/// </summary>
public partial class EasyComponentDemo(GameWindowSettings? s = null, NativeWindowSettings? n = null) : EasyWindowOpenTK(s, n) {
#pragma warning disable CS0649, CS8618, IDE0044, CA1822
    [EzButton("Test Button", OnClick = "TestButtonOnClick")]
    private EzButton TestButton;
    private void TestButtonOnClick(EzButton sender) {
        Console.WriteLine($"Test Button Clicked! {sender}");
    }
#pragma warning restore CS0649, CS8618, IDE0044, CA1822

    private readonly EzPanel Panel = new();
    private readonly EzButton BtnOk = new("OK");
    private readonly EzButton BtnCancel = new("Cancel");

    [ImGuiWindow(AutoBegin = true, AutoEnd = true, Id = "MainWindow")]
    protected void MainWindow(FrameEventArgs args) {
        Panel.Render();
    }

    protected override void ComponentSetup() {
        BtnOk.OnClick += BtnOk_OnClick;
        BtnCancel.OnClick += BtnCancel_OnClick;
        Panel.AddChild(BtnOk);
        Panel.AddChild(TestButton);
    }

    private void BtnCancel_OnClick(EzButton sender) {
        Panel.RemoveChild(sender);
        Panel.AddChild(BtnOk);
    }

    private void BtnOk_OnClick(EzButton sender) {
        Panel.RemoveChild(sender);
        Panel.AddChild(BtnCancel);
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
    }
}

public partial class EasyComponentDemo {
    public static void RunDemo() {
        var gws = GameWindowSettings.Default;
        gws.UpdateFrequency = 30;
        var nws = NativeWindowSettings.Default;
        nws.TransparentFramebuffer = true;
        nws.ClientSize = new OpenTK.Mathematics.Vector2i(1280, 720);
        var window = new EasyComponentDemo(gws, nws) {
            Title = "Component Demo"
        };
        window.Run();
        window.Dispose();
    }
}
