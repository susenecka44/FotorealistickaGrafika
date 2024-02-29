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
    /// <summary>
    /// Main function that handles the arguments - either from config file or from arguments
    /// </summary>
    public static void Main(string[] args)
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

    /// <summary>
    /// Creates an HDR image according to raytracing program 
    /// </summary>
    private static void CreateImage(int width, int height, string fileName)
    {
        // create an image in which we'll the colours from raytracing be inserted
        FloatImage fi = new FloatImage(width, height, 3);

        Camera camera = new Camera(); 
        Raytracing raytracer = new Raytracing();
        PointLight light = new PointLight(new Vector3(-1, -1, 0), new Vector3(255, 238, 237));

        
        // create a scene to be rendered & add objects to it
        List<IHittable> scene = new List<IHittable>();
       // scene.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
         scene.Add(new Sphere(new Vector3(0.5f, 0, -1), 0.5f));
        scene.Add(new Cube(new Vector3(0.5f, 0, -1), new Vector3(1f, 0, -1)));
       // scene.Add(new Sphere(new Vector3(0, 0.5f, -1), 0.5f));

        // Raytrace every pixel 
        for (int j = height - 1; j >= 0; --j)
        {
            for (int i = 0; i < width; ++i)
            {
                float u = (float)i / (width - 1);
                float v = (float)j / (height - 1);
                Ray r = camera.GetRay(u, v);
                Vector3 color = raytracer.RayColor(r, scene, light);
                float[] convertedColor = { color.X / 255.0F, color.Y / 255.0F, color.Z / 255.0F };   // R, G, B
                fi.PutPixel(i, j, convertedColor);
            }
        }

        // Implement RayColor function to determine the color of a pixel based on ray-object intersections

        fi.SaveHDR(fileName);     // HDR format is still buggy
       // fi.SavePFM(fileName);     // Works ok with the PFM format

        Console.WriteLine($"HDR image '{fileName}' is finished.");

    }

}
