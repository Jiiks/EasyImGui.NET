using EasyImGui.NET.Common;
using EasyImGui.NET.Windowing.OpenTK;
using Hexa.NET.ImGui;
using HeyRed.Mime;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using System.Numerics;
using TextCopy;

namespace EasyImGui.NET.Demos;
public partial class EasyFileExplorerDemo(GameWindowSettings? s = null, NativeWindowSettings? n = null) : EasyWindowOpenTK(s, n) {

    private const string ROOT_DIR = "C:\\";

    private readonly Dictionary<string, DirectoryInfo[]> _dirCache = [];
    private DirectoryInfo? _currentDir, _selectedDir;
    private DirectoryInfo[] GetCachedDir(string path) {
        if (_dirCache.TryGetValue(path, out DirectoryInfo[]? value)) return value;
        var di = new DirectoryInfo(path);
        try {
            var dirs = di.GetDirectories();
            _dirCache[path] = dirs;
            return dirs;
        } catch (UnauthorizedAccessException) {
            throw;
        }
    }

    private int _selectedFileIdx = 0;
    private FileInfo? _selectedFile;
    private readonly Dictionary<string, FileInfo[]> _fileCache = [];
    private FileInfo[] GetCachedFiles(DirectoryInfo di) {
        if (_fileCache.TryGetValue(di.FullName, out FileInfo[]? value)) return value;
        try {
            var files = di.GetFiles();
            _fileCache[di.FullName] = files;
            return files;
        } catch(UnauthorizedAccessException) {
            throw;
        }
    }

    [ImGuiWindow(AutoEnd = true)]
    public void MainWindow(FrameEventArgs args) {
        ImGuiWindowFlags flags = ImGuiWindowFlags.None | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoTitleBar;
        ImGui.SetNextWindowSize(new Vector2(Size.X - 20, Size.Y));
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.Begin("File Explorer###MainWindow", flags);
    }

    [ImGuiWindow(AutoBegin = true, AutoEnd = true, Id = "MainWindow")]
    public void Container(FrameEventArgs args) {
        ImGui.BeginTable("File Browser", 2);

        // Header
        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.TableHeader("Directories");
        ImGui.TableNextColumn();
        ImGui.TableHeader("Files");

        // Body
        ImGui.TableNextRow(); ImGui.TableNextColumn();

        // Directory body
        ImGui.BeginChild("Directories");
        var root = new DirectoryInfo(ROOT_DIR);
        RenderDirTree(root);
        ImGui.EndChild();

        // Files body
        ImGui.TableNextColumn();

        
        var filePreviewHeight = 220.0f;
        ImGui.BeginChild("Files", new Vector2(0, Size.Y - (filePreviewHeight - 50) * 2));
        if (_currentDir != null) {
            try {
                _selectedDir = _currentDir;
                var files = GetCachedFiles(_currentDir);
                if (files.Length <= 0) ImGui.Text("No Files");
                int fileIdx = 0;
                foreach (var file in files) {
                    if(ImGui.Selectable(file.Name, _selectedFileIdx == fileIdx)) {
                        _selectedFileIdx = fileIdx;
                        _selectedFile = file;
                    }
                    fileIdx++;
                }
            } catch (UnauthorizedAccessException e) {
                ImGui.Text($"{e.Message}");
            }
        }
        ImGui.EndChild();
        //ImGui.SetCursorPosY(ImGui.GetCursorPosY() - filePreviewHeight);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, .2f));
        ImGui.BeginChild("FilePreview", new Vector2(0, filePreviewHeight));
        ImGui.Text("Preview");
        if (_selectedFile != null) FileInfoTable(_selectedFile);
        ImGui.EndChild();
        ImGui.PopStyleColor();


        ImGui.EndTable();
    }

    private void RenderDirTree(DirectoryInfo di) {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (di.Name == ROOT_DIR) flags |= ImGuiTreeNodeFlags.DefaultOpen;
        if (_selectedDir?.Name == di.Name) flags |= ImGuiTreeNodeFlags.Selected;
        if (ImGui.TreeNodeEx(di.Name, flags)) {
            _currentDir = di;
            try {
                var subDirectories = GetCachedDir(di.FullName);
                if (subDirectories == null) return;
                foreach (var subDir in subDirectories) {
                    RenderDirTree(subDir);
                }
            } catch (UnauthorizedAccessException) {
            }

            ImGui.TreePop();
        }
    }

    protected unsafe void FileInfoTable(FileInfo fi) {
        if (fi == null) return;

        if (!ImGui.BeginTable("fitable", 2)) return;

        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.Text($"Type");
        ImGui.TableNextColumn();
        var mime = MimeTypesMap.GetMimeType(fi.Name);
        ImGui.Text($"{mime}");

        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.Text($"Size");
        ImGui.TableNextColumn();
        ImGui.Text($"{DemoUtils.FormatFileSize(fi.Length)}");

        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.Text($"Created");
        ImGui.TableNextColumn();
        ImGui.Text($"{fi.CreationTime}");

        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.Text($"Modified");
        ImGui.TableNextColumn();
        ImGui.Text($"{fi.LastWriteTime}");

        ImGui.TableNextRow(); ImGui.TableNextColumn();
        ImGui.Text($"Accessed");
        ImGui.TableNextColumn();
        ImGui.Text($"{fi.LastAccessTime}");
        bool hasPreview = false;
        //if (mime.StartsWith("image/")) {
        //    if (!_images.TryGetValue(fi.Name, out (Vector2, uint) value)) {
        //        int x, y;
        //        int channels = 0;
        //        byte* imgBytes = StbImage.Load(fi.FullName, &x, &y, ref channels, 4);
        //        var texture = Utils.CreateTexture(imgBytes, x, y);
        //        value = (new Vector2(x, y), texture);
        //        _images.Add(fi.Name, value);
        //    }
        //    ImGui.TableNextRow(); ImGui.TableNextColumn();
        //    Text("Dimensions");
        //    ImGui.TableNextColumn();
        //    var (size, image) = value;
        //    ImGui.Text($"{size.X}*{size.Y}");
        //    ImGui.TableNextRow(); ImGui.TableNextColumn();
        //    Text("Preview");
        //    ImGui.TableNextColumn();
        //    var fixedHeight = 128.0f;
        //    var aspectRatio = size.X / size.Y;
        //    var scaledWidth = fixedHeight * aspectRatio;
        //    ImGui.Image(image, new Vector2(scaledWidth, fixedHeight));
        //    hasPreview = true;
        //} else if (mime == "text/plain") {
        //    //var text = File.ReadAllText(fi.FullName);
        //    //Text(text);
        //    //hasPreview = true;
        //} else {
        //    ImGui.TableNextRow(); ImGui.TableNextColumn();
        //    Text("Preview");
        //    ImGui.TableNextColumn();
        //    Text("No preview");
        //}
        ImGui.TableNextRow(); ImGui.TableNextColumn();
        if (hasPreview) ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 110);
        var btnWidth = ImGui.CalcTextSize("Copy path to clipboard").X + 20;
        var btnSize = new Vector2(btnWidth, 30);
        if (ImGui.Button("Copy path to clipboard", btnSize)) {
            ClipboardService.SetText(fi.FullName);
        }
        ImGui.TableNextRow(); ImGui.TableNextColumn();
        if (hasPreview) ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 110 + 30);
        if (ImGui.Button("Open in explorer", btnSize)) {
            if (fi.Directory == null) return;
            Process.Start("explorer.exe", fi.Directory.FullName);
        }
        ImGui.TableNextRow(); ImGui.TableNextColumn();
        if (hasPreview) ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 110 + 60);
        if (ImGui.Button("View/Execute", btnSize)) {
            Process.Start(new ProcessStartInfo {
                FileName = fi.FullName,
                UseShellExecute = true
            });
        }
        ImGui.EndTable();

    }

}

public partial class EasyFileExplorerDemo {
    public static void RunDemo() {
        var gws = GameWindowSettings.Default;
        gws.UpdateFrequency = 30;
        var nws = NativeWindowSettings.Default;
        nws.TransparentFramebuffer = true;
        nws.ClientSize = new OpenTK.Mathematics.Vector2i(1280, 720);
        var window = new EasyFileExplorerDemo(gws, nws) {
            Title = "File Explorer ListBox Demo"
        };
        window.Run();
        window.Dispose();
    }
}
