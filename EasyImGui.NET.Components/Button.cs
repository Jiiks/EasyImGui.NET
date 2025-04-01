using Hexa.NET.ImGui;
namespace EasyImGui.NET.Components;

/// <summary>
/// This is currently just testing different ways of doing components.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EzButtonAttribute(string text) : Attribute {
    public string Text = text;
    public string? OnClick;
}

/// <summary>
/// This is currently just testing different ways of doing components.
/// </summary>
public class EzButton(string text) : IEzComponent {
    public event Action<EzButton>? OnClick;
    public string Text { get; set; } = text;

    public void Render() {
        if(ImGui.Button(Text)) {
            OnClick?.Invoke(this);
        }
    }
}
