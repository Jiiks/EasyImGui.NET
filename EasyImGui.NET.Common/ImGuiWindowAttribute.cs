namespace EasyImGui.NET.Common;
[AttributeUsage(AttributeTargets.Method)]
public class ImGuiWindowAttribute : Attribute {
    public bool AutoBegin;
    public bool AutoEnd;
    public string? Title;
    public string? Id;
}
