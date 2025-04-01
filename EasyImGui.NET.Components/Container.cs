namespace EasyImGui.NET.Components;
/// <summary>
/// This is currently just testing different ways of doing components.
/// </summary>
public class Container : IEzComponent {
    public List<IEzComponent> Children { get; set; } = [];
    private readonly Queue<IEzComponent> _removeQueue = [];
    private readonly Queue<IEzComponent> _addQueue = [];

    public void AddChild(IEzComponent child) => _addQueue.Enqueue(child);
    public void RemoveChild(IEzComponent child) => _removeQueue.Enqueue(child);

    public virtual void Render() {
        while (_addQueue.Count > 0) Children.Add(_addQueue.Dequeue());
        while (_removeQueue.Count > 0) Children.Remove(_removeQueue.Dequeue());
    }
}
