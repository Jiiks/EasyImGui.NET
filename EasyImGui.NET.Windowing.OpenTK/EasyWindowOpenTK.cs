
using EasyImGui.NET.Common;
using Hexa.NET.ImGui.Backends.GLFW;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.ImGui;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Reflection;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Resources;

namespace EasyImGui.NET.Windowing.OpenTK;

internal struct ImGuiAction {
    public Action<FrameEventArgs> Action;
    public ImGuiWindowAttribute? ImGuiWindowAttribute;
}

public unsafe class EasyWindowOpenTK(GameWindowSettings? gameWindowSettings = null, NativeWindowSettings? nativeWindowSettings = null) :
    GameWindow(gameWindowSettings ?? GameWindowSettings.Default, nativeWindowSettings ?? NativeWindowSettings.Default) {

    /// <summary>GLFW pointer of Window </summary>
    public GLFWwindow* GLFWPtr => (GLFWwindow*)WindowPtr;
    public Color4 ClearColor = Color4.Aquamarine;
    public ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
    public string GlslVersion = "#version 150";

    private List<ImGuiAction> _imguiWindows = [];
    public int TestInt = 0;

    protected override void OnLoad() {
        ImGui.SetCurrentContext(ImGui.CreateContext());
        ImGuiImplGLFW.SetCurrentContext(ImGui.GetCurrentContext());
        ImGuiImplGLFW.InitForOpenGL(GLFWPtr, true);
        ImGuiImplOpenGL3.SetCurrentContext(ImGui.GetCurrentContext());
        ImGuiImplOpenGL3.Init(GlslVersion);

        var methods = ActionGen.GetMethods(this);
        foreach (var method in methods) {
            _imguiWindows.Add(new() {
                Action = ActionGen.CreateAction(this, method),
                ImGuiWindowAttribute = method.GetCustomAttribute<ImGuiWindowAttribute>()
            });
        }

        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        // Process fields
        EasySetters.SetColorsDefs(this, fields);
        EasySetters.SetColorDefs(this, fields);

        // Process instanced res containers
        foreach (var field in fields) {
            if (!typeof(IEasyDefines).IsAssignableFrom(field.FieldType)) continue;

            var dFields = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (dFields.Length <= 0) continue;

            var inst = field.GetValue(this);
            EasySetters.SetColorDefs(inst, dFields);
        }

        // TODO simplify(?) and move elsewhere. Probably into EasyTextureOpenTK
        var mainAssembly = Assembly.GetEntryAssembly();
        if (mainAssembly == null) return;
        var mainAssemblyName = mainAssembly.GetName().Name;
        var resourceNames = mainAssembly.GetManifestResourceNames();
        foreach (var field in fields) {
            var attribute = field.GetCustomAttribute<TextureDefAttribute>();
            if (attribute == null) continue;
            if(attribute.ResourceName == null) continue;
            var fullName = $"{mainAssemblyName}.Resources.{attribute.ResourceName}";

            if (!resourceNames.Contains(fullName)) continue;

            using Stream? resourceStream = mainAssembly.GetManifestResourceStream(fullName);
            if (resourceStream == null) continue;
            using MemoryStream memoryStream = new();
            resourceStream.CopyTo(memoryStream);

            var bytes = memoryStream.ToArray();
            var tex = EasyTextureOpenTK.CreateTexture(bytes, attribute.Width, attribute.Height, attribute.ResourceName);
            field.SetValue(this, tex);
        }


    }

    /// <inheritdoc/>
    protected override void OnRenderFrame(FrameEventArgs args) {
        if (IsExiting) return;
        GL.ClearColor(ClearColor);
        GL.Clear(ClearBufferMask);
        TestInt++;

        ImGuiImplOpenGL3.NewFrame();
        ImGuiImplGLFW.NewFrame();
        ImGui.NewFrame();

        foreach (var imguiWin in _imguiWindows) {
            var action = imguiWin.Action;
            var attr = imguiWin.ImGuiWindowAttribute;
            if (attr != null) {
                if (attr.AutoBegin) ImGui.Begin($"{attr.Title}###{attr.Id}");
                action(args);
                if (attr.AutoEnd) ImGui.End();
            }
            //imguiWin(args);
        }

        ImGui.Render();
        ImGui.EndFrame();
        ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());
        ImGui.UpdatePlatformWindows();
        ImGui.RenderPlatformWindowsDefault();

        SwapBuffers();
    }

    /// <inheritdoc/>
    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        ImGuiImplOpenGL3.Shutdown();
        ImGuiImplGLFW.Shutdown();
        ImGui.DestroyContext();
    }

}

internal static class ActionGen {

    internal static IEnumerable<MethodInfo> GetMethods(object from) {
        return from.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        ).Where(method => method.GetCustomAttribute<ImGuiWindowAttribute>() != null);
    }

    internal static Action<FrameEventArgs> CreateAction(object from, MethodInfo methodInfo) {
        return (Action<FrameEventArgs>)Delegate.CreateDelegate(typeof(Action<FrameEventArgs>), from, methodInfo);
    }

}
