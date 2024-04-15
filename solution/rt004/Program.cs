using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using System;
using OpenTK.Mathematics;

namespace rt004;
public class Options
{
    [Option('w', "width", Required = false, HelpText = "Image width.")]
    public int Width { get; set; } = 600; // Default value = 600

    [Option('h', "height", Required = false, HelpText = "Image height.")]
    public int Height { get; set; } = 400; // Default value = 400

    [Option('f', "file", Required = false, HelpText = "Output file name.")]
    public string FileName { get; set; } = "picture.pfm"; // Default value = picture.pfm

    [Option('c', "config", Required = false, HelpText = "Configuration file path.")]
    public string ConfigFile { get; set; } = "config.json"; // Deafault value = config.json
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

                try
                {
                    // sceneConfig cant be null - checked before
                   ProcessAndGenerateImage(ref sceneConfig);

                }
                catch
                {
                    Console.WriteLine("There was an error while generating your image, please check your settings.");
                }
            });
    }


    /// <summary>
    /// Creates an image according to json scenes
    /// </summary>
    private static void ProcessAndGenerateImage(ref SceneConfig config)
    {
        ICamera camera = new PerspectiveCamera(new Vector3d(config.CameraSettings.Position[0], config.CameraSettings.Position[1], config.CameraSettings.Position[2]), config.Width, config.Height, config.CameraSettings.FOVAngle, new Vector3d(config.CameraSettings.Direction[0], config.CameraSettings.Direction[1], config.CameraSettings.Direction[2]));
        IRayTracer raytracer = new Raytracer(new Vector3d(config.CameraSettings.BackgroundColor[0], config.CameraSettings.BackgroundColor[1], config.CameraSettings.BackgroundColor[2]), config.AlgorithmSettings.MaxDepth, config.AlgorithmSettings.MinimalPerformance, config.AlgorithmSettings.ShadowsEnabled, config.AlgorithmSettings.ReflectionsEnabled, config.AlgorithmSettings.RefractionsEnabled);
        List<LightSource> lightSources = new List<LightSource>();
        List<IHittable> sceneObjects = new List<IHittable>();

        // Loading materials
        Dictionary<string, ObjectMaterial> loadedMaterials = new Dictionary<string, ObjectMaterial>();
        foreach (var mat in config.Materials)
        {
            loadedMaterials[mat.Name] = new ObjectMaterial(mat.Color, mat.Ambient, mat.Diffuse, mat.Specular, mat.Shininess, mat.Reflectivity, mat.Refractivity);
        }

        // Loading light sources
        foreach (var light in config.Lights)
        {
            if (light.Type == "Point")
            {
                lightSources.Add(new PointLight(new Vector3d(light.Position[0], light.Position[1], light.Position[2]), new Vector3d(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
            }
            else if (light.Type == "Ambient")
            {
                lightSources.Add(new AmbientLight(new Vector3d(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
            }
        }

        // Adding objects to the scene based on the loaded materials
        foreach (var obj in config.ObjectsInScene)
        {
            foreach (var shape in obj.BasicShapes)
            {
                AddSceneObject(shape, loadedMaterials, sceneObjects);
            }
        }

        // Load Aliasing algorithm
        IAliasingAlgorithm aliasAlgorithm = GetAliasingAlgorithm(config.AlgorithmSettings.AntiAliasing);

        // Generate the picture itself
        FloatImage fi = GeneratePicture(ref aliasAlgorithm,  config.Width,  config.Height, ref camera, ref raytracer, ref sceneObjects, ref lightSources);
        SaveFile(config.FileName, fi);
    }

    private static void AddSceneObject(PrimitiveObject obj, Dictionary<string, ObjectMaterial> loadedMaterials, List<IHittable> scene)
    {
        ObjectMaterial material = loadedMaterials[obj.Material];
        IHittable hittable = null;
        switch (obj.Type.ToLower())
        {
            case "sphere":
                hittable = new Sphere(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), obj.Radius, material);
                break;
            case "cube":
                hittable = new Cube(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3d(obj.Size[0], obj.Size[1], obj.Size[2]), material, obj.RotationAngle);
                break;
            case "plane":
                hittable = new Plane(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3d(obj.Normal[0], obj.Normal[1], obj.Normal[2]), material);
                break;
            case "cylinder":
                hittable = new Cylinder(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), obj.Height, obj.Radius, material);
                break;
            default:
                throw new InvalidOperationException("Unknown object type.");
        }
        scene.Add(hittable);
    }

    private static IAliasingAlgorithm GetAliasingAlgorithm(string aliasType)
    {
        switch (aliasType)
        {
            case "NoAliasing":
                return new NoAliasing();
            case "JitteredSamplingAliasing":
                return new JitteredSamplingAliasing();
            case "SupersamplingAliasing":
                return new SupersamplingAliasing();
            case "HammersleyAliasing":
                return new HammersleyAliasing();
            case "CorrelatedMultiJitteredAliasing":
                return new CorrelatedMultiJitteredAliasing();
            default:
                return new NoAliasing();
        }
    }

    private static FloatImage GeneratePicture(ref IAliasingAlgorithm aliasAlgorithm, int width, int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        
        // image depth set to 3 by default
        FloatImage fi = new FloatImage(width, height, 3);

        // Raytrace every pixel with jittering anti-aliasing
        for (int j = height - 1; j >= 0; --j)
        {
            for (int i = 0; i < width; ++i)
            {
                Vector3d color = new Vector3d(0, 0, 0); // Initialize color accumulator

                // antialias
                aliasAlgorithm.PixelAlias(ref color, ref i, ref j, ref width, ref height, ref camera, ref raytracer, ref algorithmSettings, ref scene, ref lightSources);

                float[] convertedColor = { (float)color.X / 255.0F, (float)color.Y / 255.0F, (float)color.Z / 255.0F };   // R, G, B
                fi.PutPixel(i, j, convertedColor);
            }
        }
        return fi;
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
