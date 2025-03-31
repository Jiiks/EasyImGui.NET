using System.Diagnostics;

namespace EasyImGui.NET.Common;
public static class EasyDebugger {

    private static readonly string _startTime = $"{DateTime.Now:HH-mm-ss_fff}";
    private static readonly HashSet<string> _timeStamps = [];
    private static readonly HashSet<string> _log = [];

    public static void WriteLine(string module, string message, bool once = true) {
        var output = $"[EasyImGui.{module}] {message}";
        if(once) {
            if (_log.Contains(output)) return;
            _log.Add(output);
        }
        var ts = $"[{DateTime.Now:HH:mm:ss.fff}]";
        _timeStamps.Add(ts);
        Debug.WriteLine($"{ts}{output}");
        Console.WriteLine($"{ts}{output}");
    }

    public static void DumpLog(string filePath) {
        if (Directory.Exists(filePath)) {
            filePath = Path.Combine(filePath, $"{_startTime}.EasyImGui.log");
        }

        try {
            if (_timeStamps.Count != _log.Count) {
                File.WriteAllLines(filePath, _log);
                return;
            }

            File.WriteAllLines(filePath, _timeStamps.Zip(_log, (timestamp, log) => $"{timestamp}{log}"));
        } catch(Exception e) {
            Debug.WriteLine($"Failed to dump log to: {filePath} - {e.Message}");
            Console.WriteLine($"Failed to dump log to: {filePath} - {e.Message}");
        }

    }

}
