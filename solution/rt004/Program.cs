using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;
using System;

namespace rt004;
public class Options
{
    [Option('w', "width", Required = false, HelpText = "Image width.")]
    public int Width { get; set; } = 600; // Default value = 600

    [Option('h', "height", Required = false, HelpText = "Image height.")]
    public int Height { get; set; } = 400; // Default value = 400

    [Option('f', "file", Required = false, HelpText = "Output file name.")]
    public string FileName { get; set; } = "picture.hdr";

    [Option('c', "config", Required = false, HelpText = "Configuration file path.")]
    public string? ConfigFile { get; set; } = "config.json";
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
                SceneConfig? sceneConfig = null;
                if (!string.IsNullOrEmpty(opts.ConfigFile))
                {
                    try
                    {
                        string jsonText = System.IO.File.ReadAllText(opts.ConfigFile);
                        sceneConfig = JsonSerializer.Deserialize<SceneConfig>(jsonText);
                        if (sceneConfig == null) throw new Exception("Failed to load scene configuration.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading config file: {ex.Message}");
                        return;
                    }
                }

                if (sceneConfig != null)
                {
                    ImageManaging(sceneConfig.Width, sceneConfig.Height, sceneConfig.FileName, sceneConfig.Materials, sceneConfig.ObjectsInScene, sceneConfig.Lights, sceneConfig.CameraSettings, sceneConfig.AlgorithmSettings);
                }
                else
                {
                    ImageManaging(opts.Width, opts.Height, opts.FileName, new List<Material>(), new List<SceneObject>(), new List<Light>(), new CameraSettings(), new AlgorithmSettings());
                }
            });
    }

    /// <summary>
    /// Creates an HDR image according to raytracing program 
    /// </summary>
    private static void ImageManaging(int width, int height, string fileName, List<Material> materials, List<SceneObject> objectsInScene, List<Light> lights, CameraSettings cameraSettings, AlgorithmSettings algorithmSettings)
    {
        {
            Camera camera = new PerspectiveCamera(new Vector3(cameraSettings.Position[0], cameraSettings.Position[1], cameraSettings.Position[2]), width, height, cameraSettings.FOVAngle, new Vector3(cameraSettings.Direction[0], cameraSettings.Direction[1], cameraSettings.Direction[2]));
            Raytracer raytracer = new Raytracer(new Vector3(cameraSettings.BackgroundColor[0], cameraSettings.BackgroundColor[1], cameraSettings.BackgroundColor[2]), algorithmSettings.MaxDepth, algorithmSettings.MinimalPerformance, algorithmSettings.ShadowsEnabled, algorithmSettings.ReflectionsEnabled, algorithmSettings.RefractionsEnabled);
            List<LightSource> lightSources = new List<LightSource>();
            List<IHittable> scene = new List<IHittable>();

            // Loading materials
            Dictionary<string, ObjectMaterial> loadedMaterials = new Dictionary<string, ObjectMaterial>();
            foreach (var mat in materials)
            {
                loadedMaterials[mat.Name] = new ObjectMaterial(mat.Color, mat.Ambient, mat.Diffuse, mat.Specular, mat.Shininess, mat.Reflectivity, mat.Refractivity);
            }

            // Loading light sources
            foreach (var light in lights)
            {
                if (light.Type == "PointLight")
                {
                    lightSources.Add(new PointLight(new Vector3(light.Position[0], light.Position[1], light.Position[2]), new Vector3(light.Color[0], light.Color[1], light.Color[2])));
                }
                else if (light.Type == "AmbientLight")
                {
                    lightSources.Add(new AmbientLight(new Vector3(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
                }
            }

            // Adding objects to the scene based on the loaded materials
            foreach (var obj in objectsInScene)
            {
                ObjectMaterial material = loadedMaterials[obj.Material];
                switch (obj.Type.ToLower())
                {
                    case "sphere":
                        scene.Add(new Sphere(new Vector3(obj.Position[0], obj.Position[1], obj.Position[2]), obj.Radius, material));
                        break;
                    case "cube":
                        scene.Add(new Cube(new Vector3(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3(obj.Size[0], obj.Size[1], obj.Size[2]), material, obj.RotationAngle));
                        break;
                    case "plane":
                        scene.Add(new Plane(new Vector3(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3(obj.Normal[0], obj.Normal[1], obj.Normal[2]), material));
                        break;
                }
            }

            FloatImage fi = GeneratePicture(ref width, ref height, ref camera, ref raytracer, ref algorithmSettings, ref scene, ref lightSources);
            SaveFile(fileName, fi);
        }
    }


    private static FloatImage GeneratePicture(ref int width, ref int height, ref Camera camera, ref Raytracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        // image depth set to 3 by default
        FloatImage fi = new FloatImage(width, height, 3);

        // Raytrace every pixel with jittering anti-aliasing
        for (int j = height - 1; j >= 0; --j)
        {
            for (int i = 0; i < width; ++i)
            {
                Vector3 color = new Vector3(0, 0, 0); // Initialize color accumulator

                if (algorithmSettings.AntiAliasing)
                {
                    GetColorPixelWithAA(ref color, ref i, ref j, ref width, ref height, ref camera, ref raytracer, ref algorithmSettings, ref scene, ref lightSources);
                }
                else
                {
                    GetColorPixelWithoutAA(ref color, ref i, ref j, ref width, ref height, ref camera, ref raytracer, ref algorithmSettings, ref scene, ref lightSources);
                }

                float[] convertedColor = { color.X / 255.0F, color.Y / 255.0F, color.Z / 255.0F };   // R, G, B
                fi.PutPixel(i, j, convertedColor);
            }
        }
        return fi;
    }
    private static void GetColorPixelWithoutAA(ref Vector3 color, ref int i, ref int j, ref int width, ref int height, ref Camera camera, ref Raytracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources )
    {     
        float u = (float)i / (width - 1);
        float v = (float)j / (height - 1);
        Ray r = camera.GetRay(u, v);
        color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
    }
    private static void GetColorPixelWithAA(ref Vector3 color, ref int i, ref int j, ref int width, ref int height, ref Camera camera, ref Raytracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        Random random = new Random();

        for (int s = 0; s < algorithmSettings.SamplesPerPixel; ++s)
        {
            // Apply jittering: add a small random amount to each u and v coordinate
            float u = ((float)i + (float)random.NextDouble()) / (width - 1);
            float v = ((float)j + (float)random.NextDouble()) / (height - 1);

            Ray r = camera.GetRay(u, v);
            color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        }

        // Average the accumulated color by the number of samples and convert to [0, 1] range
        color /= algorithmSettings.SamplesPerPixel;
    }

    private static void SaveFile(string fileName, FloatImage image)
    {
        // Check the file extension
        string extension = Path.GetExtension(fileName).ToLower();

        switch (extension)
        {
            case ".hdr":
                // HDR format is still buggy
                image.SaveHDR(fileName); 
                Console.WriteLine($"HDR image '{fileName}' is finished.");
                break;
            case ".pfm":
                image.SavePFM(fileName);
                Console.WriteLine($"PFM image '{fileName}' is finished.");
                break;
            default:
                Console.WriteLine("Unrecognized file format, saving in default PFM format.");
                image.SavePFM(fileName); // Default to PFM format if the extension is not recognized
                Console.WriteLine($"PFM image '{fileName}' is finished.");
                break;
        }
    }
}
