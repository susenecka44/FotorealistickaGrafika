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
                    ProcessAndGenerateImage(sceneConfig.Width, sceneConfig.Height, sceneConfig.FileName, sceneConfig.Materials, sceneConfig.ObjectsInScene, sceneConfig.Lights, sceneConfig.CameraSettings, sceneConfig.AlgorithmSettings);
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
    private static void ProcessAndGenerateImage(int width, int height, string fileName, List<Material> materials, List<SceneObject> objectsInScene, List<Light> lights, CameraSettings cameraSettings, AlgorithmSettings algorithmSettings)
    {
        
        ICamera camera = new PerspectiveCamera(new Vector3d(cameraSettings.Position[0], cameraSettings.Position[1], cameraSettings.Position[2]), width, height, cameraSettings.FOVAngle, new Vector3d(cameraSettings.Direction[0], cameraSettings.Direction[1], cameraSettings.Direction[2]));
        IRayTracer raytracer = new Raytracer(new Vector3d(cameraSettings.BackgroundColor[0], cameraSettings.BackgroundColor[1], cameraSettings.BackgroundColor[2]), algorithmSettings.MaxDepth, algorithmSettings.MinimalPerformance, algorithmSettings.ShadowsEnabled, algorithmSettings.ReflectionsEnabled, algorithmSettings.RefractionsEnabled);
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
                lightSources.Add(new PointLight(new Vector3d(light.Position[0], light.Position[1], light.Position[2]), new Vector3d(light.Color[0], light.Color[1], light.Color[2])));
            }
            else if (light.Type == "AmbientLight")
            {
                lightSources.Add(new AmbientLight(new Vector3d(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
            }
        }

        // Adding objects to the scene based on the loaded materials
        foreach (var obj in objectsInScene)
        {
            ObjectMaterial material = loadedMaterials[obj.Material];
            switch (obj.Type.ToLower())
            {
                case "sphere":
                    scene.Add(new Sphere(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), obj.Radius, material));
                    break;
                case "cube":
                    scene.Add(new Cube(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3d(obj.Size[0], obj.Size[1], obj.Size[2]), material, obj.RotationAngle));
                    break;
                case "plane":
                    scene.Add(new Plane(new Vector3d(obj.Position[0], obj.Position[1], obj.Position[2]), new Vector3d(obj.Normal[0], obj.Normal[1], obj.Normal[2]), material));
                    break;
            }
        }

        // Load Aliasing algorithm
        IAliasingAlgorithm aliasAlgorithm;
        switch (algorithmSettings.AntiAliasing)
        {
            case "NoAliasing":
                aliasAlgorithm = new NoAliasing();
                break;
            case "JitteredSamplingAliasing":
                aliasAlgorithm = new JitteredSamplingAliasing();
                break;
            case "SupersamplingAliasing":
                aliasAlgorithm = new SupersamplingAliasing();
                break;
            case "HammersleyAliasing":
                aliasAlgorithm = new HammersleyAliasing();
                break;
            case "CorrelatedMultiJitteredAliasing":
                aliasAlgorithm = new CorrelatedMultiJitteredAliasing();
                break;
            default:
                aliasAlgorithm = new JitteredSamplingAliasing();
                break;
        }

        // Generate the picture itself
        FloatImage fi = GeneratePicture(ref aliasAlgorithm, ref width, ref height, ref camera, ref raytracer, ref algorithmSettings, ref scene, ref lightSources);
        SaveFile(fileName, fi);
        
    }

    private static FloatImage GeneratePicture(ref IAliasingAlgorithm aliasAlgorithm, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
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
