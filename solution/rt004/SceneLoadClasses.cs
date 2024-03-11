using rt004;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Material
{
    public string Name { get; set; }
    public float[] Color { get; set; }
    public float Ambient { get; set; }
    public float Diffuse { get; set; }
    public float Specular { get; set; }
    public float Shininess { get; set; }
    public float Reflectivity { get; set; }
    public float Refractivity { get; set; }
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

public class AlgorithmSettings
{
    public bool ReflectionsEnabled { get; set; }
    public bool ShadowsEnabled { get; set; }
    public bool RefractionsEnabled { get; set; }
    public int MaxDepth { get; set; }
    public int SamplesPerPixel { get; set; }
    public float MinimalPerformance { get; set; }
}


public class SceneConfig : Options
{
    public List<Material> Materials { get; set; }
    public List<SceneObject> ObjectsInScene { get; set; }
    public List<Light> Lights { get; set; }
    public CameraSettings CameraSettings { get; set; }
    public AlgorithmSettings AlgorithmSettings { get; set; }
}

