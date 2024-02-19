using CommandLine;
using System.Text.Json;
using Util;

namespace rt004;


class Options
{
    [Option('w', "width", Required = false, HelpText = "Image width.")]
    public int Width { get; set; } = 600; // Default value = 600

    [Option('h', "height", Required = false, HelpText = "Image height.")]
    public int Height { get; set; } = 400; // Default value = 400

    [Option('f', "file", Required = false, HelpText = "Output file name.")]
    public string FileName { get; set; } = "picture.pfm";

    [Option('c', "config", Required = false, HelpText = "Configuration file path.")]
    public string ConfigFile { get; set; }
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

        float[] color1 = { 0.0f, 0.0f, 1.0f }; // Blue
        float[] color2 = { 1.0f, 0.0f, 0.0f }; // Red
        float[] white = { 1.0f, 1.0f, 1.0f }; // White


        // Gradient
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float ratio = (float)x / (width - 1); 

                float[] mixedColor = {
                color1[0] * (1 - ratio) + color2[0] * ratio,
                color1[1] * (1 - ratio) + color2[1] * ratio,
                color1[2] * (1 - ratio) + color2[2] * ratio,
            };

                fi.PutPixel(x, y, mixedColor);
            }
        }

        // Draw white circles

        int centerX = width / 2;
        int centerY = height / 2;
        int outlineWidth = 1;
        for (int i = 1; i < width; i += 10)
        {
            int radius = i;
            for (int y = centerY - radius - outlineWidth; y <= centerY + radius + outlineWidth; y++)
            {
                for (int x = centerX - radius - outlineWidth; x <= centerX + radius + outlineWidth; x++)
                {
                    double distanceFromCenter = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                    if (distanceFromCenter >= radius - outlineWidth && distanceFromCenter <= radius + outlineWidth)
                    {
                        fi.PutPixel(x, y, white);
                    }
                }
            }
        }

        fi.SaveHDR(fileName);     // HDR format is still buggy
       // fi.SavePFM(fileName);     // Works ok with the PFM format

        Console.WriteLine($"HDR image '{fileName}' is finished.");

    }
}
