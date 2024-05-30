using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using System;
using OpenTK.Mathematics;
using System.Runtime.Intrinsics.X86;

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
                    if (sceneConfig != null)
                    {
                        // sceneConfig cant be null - checked before, but I dont want any warnings :]
                        ProcessAndGenerateImage(ref sceneConfig);
                    }

                }
                catch
                {
                    Console.WriteLine("There was an error while generating your image, please check your settings.");
                }
            });
    }


    /// <summary>
    /// Loads the scene configuration and generates the image
    /// </summary>
    /// <param name="config"> SceneConfig on which you want to generate the image </param>
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
            ITexture texture;
            switch (mat.Texture.ToLower())
            {
                case "none":
                    texture = new SolidTexture(new Vector3d(mat.Color[0], mat.Color[1], mat.Color[2]));
                    break;
                case "solid":
                    texture = new SolidTexture(new Vector3d(mat.Color[0], mat.Color[1], mat.Color[2]));
                    break;
                case "checker":
                    texture = new CheckerTexture(new Vector3d(mat.Color[0], mat.Color[1], mat.Color[2]), new Vector3d(mat.SecondaryColor[0], mat.SecondaryColor[1], mat.SecondaryColor[2]), mat.TextureCoef);
                    break;
                case "wood":
                    texture = new WoodTexture(new Vector3d(mat.Color[0], mat.Color[1], mat.Color[2]), new Vector3d(mat.SecondaryColor[0], mat.SecondaryColor[1], mat.SecondaryColor[2]), mat.TextureCoef);
                    break;
                default:
                    texture = new SolidTexture(new Vector3d(mat.Color[0], mat.Color[1], mat.Color[2]));
                    break;
            }
                loadedMaterials[mat.Name] = new ObjectMaterial(mat.Color, mat.SecondaryColor, texture, mat.Ambient, mat.Diffuse, mat.Specular, mat.Shininess, mat.Reflectivity, mat.Refractivity);
        }

        // Loading light sources
        foreach (var light in config.Lights)
        {
            if (light.Type == "PointLight")
            {
                lightSources.Add(new PointLight(new Vector3d(light.Position[0], light.Position[1], light.Position[2]), new Vector3d(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
            }
            else if (light.Type == "AmbientLight")
            {
                lightSources.Add(new AmbientLight(new Vector3d(light.Color[0], light.Color[1], light.Color[2]), light.Intensity));
            }
        }

        // load the object prefabs:
        Dictionary<string, List<PrimitiveObject>> prefabs = new Dictionary<string, List<PrimitiveObject>>();
        foreach (var prefab in config.ObjectsInScene)
        {
            List<PrimitiveObject> components = new List<PrimitiveObject>();
            foreach (var shape in prefab.BasicShapes)
            {
                if (prefabs.ContainsKey(shape.Type))
                {
                    foreach (var subshape in prefabs[shape.Type])
                    {
                        components.Add(subshape);
                    }
                }
                components.Add(shape);
            }
            prefabs.Add(prefab.Name, components);
        }


        // Adding objects to the scene based on the loaded materials
        foreach (var obj in config.Scene)
        {
            if (prefabs.ContainsKey(obj.Type))
            {
                foreach (var shape in prefabs[obj.Type])
                {
                    AddSceneObject(shape, loadedMaterials, sceneObjects, obj);
                }
            }
        }

        // Load Aliasing algorithm
        IAliasingAlgorithm aliasAlgorithm = GetAliasingAlgorithm(config.AlgorithmSettings.AntiAliasing);

        // Generate the picture itself
        FloatImage fi = GeneratePicture(aliasAlgorithm, config.Width, config.Height, camera, raytracer, config.AlgorithmSettings, sceneObjects, lightSources);
        SaveFile(config.FileName, fi);
    }

    /// <summary>
    /// Adds a scene object to the scene based on parameters
    /// </summary>
    /// <param name="obj"> object type </param>
    /// <param name="loadedMaterials"> list of materials for the scene </param>
    /// <param name="scene"> list of current scene objects </param>
    /// <param name="parent"> hierarchy parent of the object </param>
    /// <exception cref="InvalidOperationException"> unknown object type passed to function </exception>
    private static void AddSceneObject(PrimitiveObject obj, Dictionary<string, ObjectMaterial> loadedMaterials, List<IHittable> scene, ObjectInScene parent)
    {
        ObjectMaterial material = loadedMaterials[obj.Material];
        IHittable hittable;

        // Scale and rotate position
        Vector3d scaledPosition = new Vector3d(obj.Position[0] * parent.Scale[0], obj.Position[1] * parent.Scale[1], obj.Position[2] * parent.Scale[2]);
        Vector3d rotatedPosition = RotateVector(scaledPosition, new Vector3d(parent.Rotation[0], parent.Rotation[1], parent.Rotation[2]));
        Vector3d objPosition = rotatedPosition + new Vector3d(parent.Position[0], parent.Position[1], parent.Position[2]);

        switch (obj.Type.ToLower())
        {
            case "sphere":
                double scaledRadius = obj.Radius * parent.Scale[0];
                hittable = new Sphere(objPosition, scaledRadius, material);
                break;
            case "cube":
                Vector3d objSize = new Vector3d(obj.Size[0] * parent.Scale[0], obj.Size[1] * parent.Scale[1], obj.Size[2] * parent.Scale[2]);
                hittable = new Cube(objPosition, objSize, material, obj.RotationAngle + parent.Rotation[1]);
                break;
            case "plane":
                Vector3d objNormal = RotateVector(new Vector3d(obj.Normal[0], obj.Normal[1], obj.Normal[2]), new Vector3d(parent.Rotation[0], parent.Rotation[1], parent.Rotation[2]));
                hittable = new Plane(objPosition, objNormal, material);
                break;
            case "cylinder":
                hittable = new Cylinder(objPosition, obj.Height, obj.Radius * parent.Scale[0], material);
                break;
            case "cone":
                hittable = new Cone(objPosition, obj.Radius, obj.MinorRadius, material); 
                break;
            default:
                throw new InvalidOperationException("Unknown object type.");
        }
        scene.Add(hittable);
    }

    /// <summary>
    /// Rotates a vector by the given angles
    /// </summary>
    /// <param name="vector"> vector to rotate </param>
    /// <param name="rotationAngles"> rotation angle </param>
    /// <returns></returns>
    private static Vector3d RotateVector(Vector3d vector, Vector3d rotationAngles)
    {
        // degrees to radians
        double radX = rotationAngles.X * Math.PI / 180.0;
        double radY = rotationAngles.Y * Math.PI / 180.0;
        double radZ = rotationAngles.Z * Math.PI / 180.0;

        // Rotation matrices for each axis
        Matrix3d rotationMatrix = Matrix3d.CreateRotationZ(radZ) * Matrix3d.CreateRotationY(radY) * Matrix3d.CreateRotationX(radX);
        Vector3d rotatedVector = vector.Transform(rotationMatrix);

        return rotatedVector;
    }

    /// <summary>
    /// Gets the aliasing algorithm based on its name in the string passed
    /// </summary>
    /// <param name="aliasType"> name of the algorithm </param>
    /// <returns> antialiasing algorithm </returns>
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

    /// <summary>
    /// Generates the picture
    /// </summary>
    /// <param name="aliasAlgorithm"> antialias algorithm used for the image </param>
    /// <param name="width"> width of the image </param>
    /// <param name="height"> height of the image </param>
    /// <param name="camera"> camera type used as viewer </param>
    /// <param name="raytracer"> raytracer type - using specific BRDF </param>
    /// <param name="algorithmSettings"> algorithm setting </param>
    /// <param name="scene"> scene objects with all their data </param>
    /// <param name="lightSources"> lights with all their data </param>
    /// <returns> FloatImage generated with the settings passed to this function </returns>
    private static FloatImage GeneratePicture(IAliasingAlgorithm aliasAlgorithm, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        Console.WriteLine("Generating... ");
        FloatImage fi = new FloatImage(width, height, 3);
        if (algorithmSettings.Paralellism)
        {
            object lockerThingy = new object();
            //  Parallel.For to spread pixel processing across multiple threads
            Parallel.For(0, height, j =>
            {
                for (int i = 0; i < width; ++i)
                {
                    Vector3d color = new Vector3d(0, 0, 0);

                    color = aliasAlgorithm.PixelAlias(color, i, j, width, height, camera, raytracer, algorithmSettings, scene, lightSources);

                    float[] convertedColor = { (float)color.X / 255.0F, (float)color.Y / 255.0F, (float)color.Z / 255.0F };
                    lock (lockerThingy)
                    {
                        fi.PutPixel(i, j, convertedColor);
                    }
                }
            });
        }
        else
        {
            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    Vector3d color = new Vector3d(0, 0, 0);

                    color = aliasAlgorithm.PixelAlias(color, i, j, width, height, camera, raytracer, algorithmSettings, scene, lightSources);

                    float[] convertedColor = { (float)color.X / 255.0F, (float)color.Y / 255.0F, (float)color.Z / 255.0F };
                    fi.PutPixel(i, j, convertedColor);
                }
            }
        }
        return fi;
    }

    /// <summary>
    /// Saves the image to a file
    /// </summary>
    /// <param name="fileName"> name of file </param>
    /// <param name="image"> the image to be saved </param>
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
