namespace EasyImGui.NET.Demos;
internal static class DemoUtils {
    const double KB = 1024.0;
    const double MB = KB * 1024.0;
    const double GB = MB * 1024.0;

    internal static string FormatFileSize(long bytes) {
        if (bytes >= GB) {
            return $"{bytes / GB:0.##} GB";
        } else if (bytes >= MB) {
            return $"{bytes / MB:0.##} MB";
        } else if (bytes >= KB) {
            return $"{bytes / KB:0.##} KB";
        } else {
            return $"{bytes} Bytes";
        }
    }


}
