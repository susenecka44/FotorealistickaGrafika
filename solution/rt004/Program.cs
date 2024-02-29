using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;

namespace rt004;


class Options
{
    [Option('w', "width", Required = false, HelpText = "Image width.")]
    public int Width { get; set; } = 600; // Default value = 600

    [Option('h', "height", Required = false, HelpText = "Image height.")]
    public int Height { get; set; } = 400; // Default value = 400

    [Option('f', "file", Required = false, HelpText = "Output file name.")]
    public string FileName { get; set; } = "picture.hdr";

    [Option('c', "config", Required = false, HelpText = "Configuration file path.")]
    public string? ConfigFile { get; set; }
}

internal class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                if (!string.IsNullOrEmpty(opts.ConfigFile))
                {
                    try
                    {
                        string jsonText = System.IO.File.ReadAllText(opts.ConfigFile);
                        var config = JsonSerializer.Deserialize<Options>(jsonText);
                        if (config != null)
                        {
                            opts = config;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading config file: {ex.Message}");
                        return;
                    }
                }
                CreateImage(opts.Width, opts.Height, opts.FileName);
            });
    }

    private static void CreateImage(int width, int height, string fileName)
    {
        FloatImage fi = new FloatImage(width, height, 3);

        float[] Blue = { 0.0f, 0.0f, 1.0f }; // Blue
        float[] Red = { 1.0f, 0.0f, 0.0f }; // Red
        float[] White = { 1.0f, 1.0f, 1.0f }; // White


        Camera camera = new Camera();
        Vector3[,] image = new Vector3[width, height];
        List<IHittable> world = new List<IHittable>();
        // Add objects to world
        world.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));

        for (int j = height - 1; j >= 0; --j)
        {
            for (int i = 0; i < width; ++i)
            {
                float u = (float)i / (width - 1);
                float v = (float)j / (height - 1);
                Ray r = camera.GetRay(u, v);
                Vector3 color = RayColor(r, world);
                image[i, j] = color;
            }
        }

        // Implement RayColor function to determine the color of a pixel based on ray-object intersections

        fi.SaveHDR(fileName);     // HDR format is still buggy
       // fi.SavePFM(fileName);     // Works ok with the PFM format

        Console.WriteLine($"HDR image '{fileName}' is finished.");

    }
}
