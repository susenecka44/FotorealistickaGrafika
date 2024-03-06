using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;

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
                    CreateImage(sceneConfig.Width, sceneConfig.Height, sceneConfig.FileName, sceneConfig.Materials, sceneConfig.ObjectsInScene, sceneConfig.Lights, sceneConfig.CameraSettings);
                }
                else
                {
                    CreateImage(opts.Width, opts.Height, opts.FileName, new List<Material>(), new List<SceneObject>(), new List<Light>(), new CameraSettings());
                }
            });
    }

    /// <summary>
    /// Creates an HDR image according to raytracing program 
    /// </summary>
    private static void CreateImage(int width, int height, string fileName, List<Material> materials, List<SceneObject> objectsInScene, List<Light> lights, CameraSettings cameraSettings)
    {
        {
            // Initialize the FloatImage, Camera, and other entities as before
            FloatImage fi = new FloatImage(width, height, 3);
            Camera camera = new Camera(new Vector3(cameraSettings.Position[0], cameraSettings.Position[1], cameraSettings.Position[2]), width, height, cameraSettings.FOVAngle, new Vector3(cameraSettings.Direction[0], cameraSettings.Direction[1], cameraSettings.Direction[2]));
            Raycasting raycaster = new Raycasting(new Vector3(cameraSettings.BackgroundColor[0], cameraSettings.BackgroundColor[1], cameraSettings.BackgroundColor[2]));
            List<LightSource> lightSources = new List<LightSource>();
            List<IHittable> scene = new List<IHittable>();

            // Loading materials
            Dictionary<string, ObjectMaterial> loadedMaterials = new Dictionary<string, ObjectMaterial>();
            foreach (var mat in materials)
            {
                loadedMaterials[mat.Name] = new ObjectMaterial(mat.Color, mat.Ambient, mat.Diffuse, mat.Specular, mat.Shininess);
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

            // Raycast every pixel 
            for (int j = height - 1; j >= 0; --j)
            {
                for (int i = 0; i < width; ++i)
                {
                    float u = (float)i / (width - 1);
                    float v = (float)j / (height - 1);
                    Ray r = camera.GetRay(u, v);
                    Vector3 color = raycaster.RayColor(r, scene, lightSources);
                    float[] convertedColor = { color.X / 255.0F, color.Y / 255.0F, color.Z / 255.0F };   // R, G, B
                    fi.PutPixel(i, j, convertedColor);
                }
            }

            // Check the file extension
            string extension = Path.GetExtension(fileName).ToLower();

            switch (extension)
            {
                case ".hdr":
                    // HDR format is still buggy
                    fi.SaveHDR(fileName); 
                    Console.WriteLine($"HDR image '{fileName}' is finished.");
                    break;
                case ".pfm":
                    fi.SavePFM(fileName);
                    Console.WriteLine($"PFM image '{fileName}' is finished.");
                    break;
                default:
                    Console.WriteLine("Unrecognized file format, saving in default PFM format.");
                    fi.SavePFM(fileName); // Default to PFM format if the extension is not recognized
                    Console.WriteLine($"PFM image '{fileName}' is finished.");
                    break;
            }
        }
    }
}
public class Material
{
    public string Name { get; set; }
    public float[] Color { get; set; }
    public float Ambient { get; set; }
    public float Diffuse { get; set; }
    public float Specular { get; set; }
    public float Shininess { get; set; }
}

public class SceneObject
{
    public string Type { get; set; }
    public float[] Position { get; set; }
    public float Radius { get; set; } // For spheres
    public float[] Size { get; set; } // For cubes 
    public float[] Normal { get; set; } // For planes
    public string Material { get; set; }
    public float RotationAngle { get; set; } // Optional, for cubes
}

public class Light
{
    public string Type { get; set; }
    public float[] Position { get; set; }
    public float[] Color { get; set; }
    public float Intensity { get; set; } // For ambient light
}


public class CameraSettings
{
    public float[] Position { get; set; }
    public float[] Direction { get; set; }
    public float[] BackgroundColor { get; set; }
    public float FOVAngle { get; set; }
}

public class SceneConfig : Options
{
    public List<Material> Materials { get; set; }
    public List<SceneObject> ObjectsInScene { get; set; }
    public List<Light> Lights { get; set; }

    public CameraSettings CameraSettings { get; set; }
}

