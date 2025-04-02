using EasyImGui.NET.Windowing.OpenTK;
using Hexa.NET.ImGui;
using HtmlAgilityPack;
using OpenTK.Windowing.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EasyImGui.NET.Windowing.HTML;

public interface IEasyViewModel {
    public EasyWindowHTML Window { get; set; }
}

public class EasyViewModel : IEasyViewModel {
    public EasyWindowHTML Window { get; set; } = null!;
}

public interface IEasyHtmlNode {
    public string Id { get; set; }
    public bool Visible { get; set; }
    public void RenderFrame(FrameEventArgs e);
    public void Render(FrameEventArgs e);
    public void Init(IEasyViewModel vm);
}

public class BaseNode : IEasyHtmlNode {
    public string Text { get; set; } = "Text";
    public string Id { set; get; } = "ID";
    public bool Visible { get; set; } = true;
    public IEasyViewModel ViewModel { get; set; }
    public required HtmlNode Node { get; set; }
    public void RenderFrame(FrameEventArgs args) {
        if (Visible) Render(args);
    }
    public virtual void Render(FrameEventArgs e) { }
    public virtual void Init(IEasyViewModel vm) {
        ViewModel = vm;
    }
}

public class Span : BaseNode {
    public override void Render(FrameEventArgs args) {
        ImGui.Text(Text);
    }

}

public class Table : BaseNode {

    private Func<EasyTableDataSource>? _binding;

    public override void Render(FrameEventArgs e) {
        if (_binding == null) {
            Console.WriteLine("Table binding is null!");
            return;
        }
        var data = _binding();
        if (ImGui.BeginTable(Id, data.Columns)) {
            ImGui.TableNextRow();
            foreach (var header in data.Headers) {
                ImGui.TableNextColumn();
                ImGui.TableHeader(header);
                if (ImGui.TableGetColumnIndex() >= data.Columns) ImGui.TableNextRow();
            }
            ImGui.TableNextRow();
            foreach (var cell in data.Cells) {
                ImGui.TableNextColumn();
                ImGui.Text(cell);
                if (ImGui.TableGetColumnIndex() >= data.Columns) ImGui.TableNextRow();
            }
        
            ImGui.EndTable();
        }
    }

    public override void Init(IEasyViewModel vm) {
        base.Init(vm);
        // get data from vm
        Console.WriteLine(Node);
        var method = vm.GetType().GetMethod(Node.GetAttributeValue<string>("datasource", string.Empty));
        Console.WriteLine(Node.GetAttributeValue<string>("datasource", string.Empty));
        if(method != null) 
            _binding = (Func<EasyTableDataSource>)method.CreateDelegate(typeof(Func<EasyTableDataSource>), vm);
    }
}

public class Button : BaseNode {
    public Action<Button>? OnClick;
    public override void Render(FrameEventArgs args) {
        if (ImGui.Button(Text)) {
            OnClick?.Invoke(this);
        }
    }

    public override void Init(IEasyViewModel vm) {
        var onClick = Node.GetAttributeValue<string>("OnClick", string.Empty);
        if (onClick != string.Empty) {
            var method = vm.GetType().GetMethod(onClick, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
                OnClick += (Action<Button>)method.CreateDelegate(typeof(Action<Button>), vm);
        }

    }
}

class Div : BaseNode {
    public string Title { get; set; } = "Window";
    public string Id { get; set; } = "Id";
    public List<IEasyHtmlNode> Children { get; set; } = [];

    //public void Init(IEasyViewModel vm) {}

    public override void Render(FrameEventArgs e) {
        //ImGuiChildFlags cf = ImGuiChildFlags.AutoResizeY | ImGuiChildFlags.AutoResizeX;
        ImGui.BeginChild(Id, new System.Numerics.Vector2(0,0));
        foreach (var node in Children) {
            node.RenderFrame(e);
        }
        ImGui.EndChild();
    }
}


public interface IEasyDataSource { }
public class  EasyTableDataSource : IEasyDataSource {
    public int Columns;
    public List<string> Headers = [];
    public List<string> Cells = [];

}

public class EasyWindowHTML {

    private Dictionary<string, IEasyHtmlNode> _nodes = [];
    public IEasyHtmlNode GetNodeById(string id) => _nodes[id];

    public EasyWindowOpenTK Window { get; private set; }
    private readonly string _htmlContent;

    private readonly HtmlNode _body;

    private readonly List<Div> _windows = [];

    private IEasyViewModel _vm;

    public EasyWindowHTML(FileInfo htmlFile, IEasyViewModel vm) : this(File.ReadAllText(htmlFile.FullName), vm) { }
    public EasyWindowHTML(string htmlContent, IEasyViewModel vm) {
        vm.Window = this;
        _htmlContent = htmlContent;
        _vm = vm;
        Window = new EasyWindowOpenTK();

        Window.RenderFrame += Window_RenderFrame;

        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        var title = doc.DocumentNode.SelectSingleNode("//head//title").InnerText;
        _body = doc.DocumentNode.SelectSingleNode("//body");
        Window.Title = title;

        foreach (var node in _body.SelectNodes("./div")) {
            var div = CreateDiv(node, vm);
            _windows.Add(div);
        }

        Window.Run();
        Window.Dispose();
    }

    private Div CreateDiv(HtmlNode node, IEasyViewModel vm) {
        var div = new Div() {
            Node = node,
            Title = node.GetAttributeValue<string>("title", "Window"),
            Id = node.Id
        };
        
        var spans = node.SelectNodes("./span");
        if (spans != null) {
            foreach (var span in spans) {
                var s = new Span() {
                    Id = span.Id,
                    Text = span.InnerText,
                    Node = span
                };
                _nodes.Add(s.Id, s);
                div.Children.Add(s);
            }
        }
        var buttons = node.SelectNodes("./button");
        if (buttons != null) {
            foreach (var btnNode in buttons) {
                var btn = new Button() {
                    Text = btnNode.InnerText,
                    Node = btnNode
                };
                btn.Init(vm);
                _nodes.Add(btn.Id, btn);
                div.Children.Add(btn);
            }
        }
        var tables = node.SelectNodes("./table");
        if (tables != null) {
            foreach (var tableNode in tables) {
                Console.WriteLine($"TABLE: {tableNode.Id} PARENT: {node.Id}");
                var table = new Table() {
                    Node = tableNode,
                    Id = tableNode.Id
                };
                if(tableNode.Id != string.Empty) _nodes.Add(table.Id, table);
                table.Init(vm);
                div.Children.Add(table);
            }
        }

        var divs = node.SelectNodes("./div");
        if (divs != null) {
            foreach (var d in divs) {
                div.Children.Add(CreateDiv(d, vm));
            }
        }

        _nodes.Add(div.Id, div);
        return div;
    }

    private void Window_RenderFrame(FrameEventArgs args) {
        foreach(var w in _windows) {
            ImGui.Begin(w.Title);
            foreach(var c in w.Children) {
                c.RenderFrame(args);
            }
            ImGui.End();
        }
    }
}
