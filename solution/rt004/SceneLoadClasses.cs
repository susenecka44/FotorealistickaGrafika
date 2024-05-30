using OpenTK.Mathematics;
using rt004;
using System.Collections.Generic;

/// <summary>
/// Classes used for loading the scene from a JSON file
/// </summary>

public class Material
{
    public string Name { get; set; } = "Default";
    public double[] Color { get; set; } = new double[] { 1.0, 1.0, 1.0 };
    public double[] SecondaryColor { get; set; } = new double[] { 0, 0, 0 };
    public double TextureCoef { get; set; } = 1;
    public string Texture { get; set; } = "None";
    public double Ambient { get; set; } = 0.1;
    public double Diffuse { get; set; } = 0.9;
    public double Specular { get; set; } = 0.5;
    public double Shininess { get; set; } = 200.0;
    public double Reflectivity { get; set; } = 0.0;
    public double Refractivity { get; set; } = 0.0;
}

public class PrimitiveObject
{
    public string Type { get; set; } = "Sphere";
    public double[] Position { get; set; } = new double[] { 0.0, 0.0, 0.0 };
    public double Radius { get; set; } = 1.0; // sphere, cylinder, torus(outer circle), cone
    public double[] Size { get; set; } = new double[] { 1.0, 1.0, 1.0 }; 
    public double[] Normal { get; set; } = new double[] { 0.0, 1.0, 0.0 }; // plane
    public string Material { get; set; } = "Default";
    public double RotationAngle { get; set; } = 0.0; // cube
    public double Height { get; set; } = 1.0;// cylinder
    public double[] Scale { get; set; } = new double[] { 1, 1, 1 }; // for nested objects
    public double[] Rotation { get; set; } = new double[] { 0, 0, 0 }; // for nested objects
    public double MinorRadius { get; set; } = 0.5; // torus, cone
}

public class Object
{
    public string Name { get; set; } = "Object";
    public List<PrimitiveObject> BasicShapes { get; set; } = new List<PrimitiveObject>();
}

public class ObjectInScene
{
    public string Type { get; set; } = "Object";
    public double[] Position { get; set; } = new double[] {0, 0 ,0 };
    public double[] Scale { get; set; } = new double[] { 1, 1, 1 };
    public double[] Rotation { get; set; } = new double[] { 0, 0, 0 };
}

public class Light
{
    public string Type { get; set; } = "PointLight";
    public double[] Position { get; set; } = new double[] { 0.0, 10.0, 0.0 };
    public double[] Color { get; set; } = new double[] { 1.0, 1.0, 1.0 }; 
    public double Intensity { get; set; } = 1.0; 
}

public class CameraSettings
{
    public double[] Position { get; set; } = new double[] { 0.0, 0.0, 5.0 };
    public double[] Direction { get; set; } = new double[] { 0.0, 0.0, -1.0 };
    public double[] BackgroundColor { get; set; } = new double[] { 0.2, 0.2, 0.2 }; 
    public double FOVAngle { get; set; } = 90.0; 
}

public class AlgorithmSettings
{
    public bool ReflectionsEnabled { get; set; } = true;
    public bool ShadowsEnabled { get; set; } = true;
    public bool RefractionsEnabled { get; set; } = true;
    public int MaxDepth { get; set; } = 5;
    public int SamplesPerPixel { get; set; } = 5;
    public double MinimalPerformance { get; set; } = 0.0;
    public string AntiAliasing { get; set; } = "JitteredSamplingAliasing";
    public string RayTracer { get; set; } = "Basic";
    public bool Paralellism { get; set; } = true;
    public double Exposure { get; set; } = 1.0f; // Default exposure

}

public class SceneConfig : Options
{
    public List<Material> Materials { get; set; } = new List<Material> { new Material() };
    public List<ObjectInScene> Scene { get; set; } = new List<ObjectInScene>();
    public List<Object> ObjectsInScene { get; set; } = new List<Object>();
    public List<Light> Lights { get; set; } = new List<Light> { new Light() };
    public CameraSettings CameraSettings { get; set; } = new CameraSettings();
    public AlgorithmSettings AlgorithmSettings { get; set; } = new AlgorithmSettings();
}
