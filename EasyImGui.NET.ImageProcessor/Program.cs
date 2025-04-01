using Hexa.NET.StbImage;

namespace EasyImGui.NET.ImageProcessor;

public unsafe class ImageProcessor {

    public static void Main(string[] args) {
        if (args.Length <= 0) {
            Console.WriteLine("No directory supplied");
            Console.ReadKey();
            return;
        }
        var wd = args[0];

        if (!Directory.Exists(wd)) {
            Console.WriteLine($"Directory does not exist: {wd}");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Working Directory: {wd}");

        var files = Directory.GetFiles(wd);
        if (files.Length <= 0) {
            Console.WriteLine($"No files in {wd}");
            Console.ReadKey();
            return;
        }

        var parentDir = Directory.GetParent(wd);
        if(parentDir == null) {
            Console.WriteLine($"Parent dir does not exist");
            Console.ReadKey();
            return;
        }

        var dirName = new DirectoryInfo(wd).Name;

        var outDir = Path.Join(parentDir.FullName, $"{dirName}_processed");
        Console.WriteLine($"Out dir: {outDir}");

        if (!Directory.Exists(outDir)) {
            try {
                Directory.CreateDirectory(outDir);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
                return;
            }
        }

        int x, y, channelsInFile = 0;

        foreach (var file in files) {

            var fn = Path.GetFileNameWithoutExtension(file);
            var outPath = Path.Join(outDir, fn);
            Console.WriteLine($"Processing {file} => {outPath}");

            try {
                var imagePtr = StbImage.Load(file, &x, &y, ref channelsInFile, 4);
                var imageData = new Span<byte>(imagePtr, x * y * channelsInFile);

                File.WriteAllBytes(outPath, imageData.ToArray());
            } catch (Exception e) { Console.WriteLine(e.Message); }
        }
        
        Console.WriteLine("All Done! Press any key to exit...");
        Console.ReadKey();
    }

}
