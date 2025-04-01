namespace EasyImGui.NET.Components;
/// <summary>
/// This is currently just testing different ways of doing components.
/// </summary>
public class EzPanel : Container {
    public override void Render() {
        base.Render();
        foreach (var child in Children) child.Render();
    }
}