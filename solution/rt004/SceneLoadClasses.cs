using rt004;

public class Material
{
    public string Name { get; set; }
    public double[] Color { get; set; }
    public double Ambient { get; set; }
    public double Diffuse { get; set; }
    public double Specular { get; set; }
    public double Shininess { get; set; }
    public double Reflectivity { get; set; }
    public double Refractivity { get; set; }
}

public class SceneObject
{
    public string Type { get; set; }
    public double[] Position { get; set; }
    public double Radius { get; set; } // For spheres
    public double[] Size { get; set; } // For cubes 
    public double[] Normal { get; set; } // For planes
    public string Material { get; set; }
    public double RotationAngle { get; set; } // Optional, for cubes
}

public class Light
{
    public string Type { get; set; }
    public double[] Position { get; set; }
    public double[] Color { get; set; }
    public double Intensity { get; set; } // For ambient light
}


public class CameraSettings
{
    public double[] Position { get; set; }
    public double[] Direction { get; set; }
    public double[] BackgroundColor { get; set; }
    public double FOVAngle { get; set; }
}

public class AlgorithmSettings
{
    public bool ReflectionsEnabled { get; set; }
    public bool ShadowsEnabled { get; set; }
    public bool RefractionsEnabled { get; set; }
    public int MaxDepth { get; set; }
    public int SamplesPerPixel { get; set; }
    public double MinimalPerformance { get; set; }
    public string AntiAliasing { get; set; }
    public string RayTracer { get; set; }
}


public class SceneConfig : Options
{
    public List<Material> Materials { get; set; }
    public List<SceneObject> ObjectsInScene { get; set; }
    public List<Light> Lights { get; set; }
    public CameraSettings CameraSettings { get; set; }
    public AlgorithmSettings AlgorithmSettings { get; set; }
}

