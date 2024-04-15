﻿using rt004;
using System.Collections.Generic;

public class Material
{
    public string Name { get; set; } = "Default";
    public double[] Color { get; set; } = new double[] { 1.0, 1.0, 1.0 };
    public double Ambient { get; set; } = 0.1;
    public double Diffuse { get; set; } = 0.9;
    public double Specular { get; set; } = 0.5;
    public double Shininess { get; set; } = 200.0;
    public double Reflectivity { get; set; } = 0.0;
    public double Refractivity { get; set; } = 0.0;
}

public class SceneObject
{
    public string Type { get; set; } = "Sphere";
    public double[] Position { get; set; } = new double[] { 0.0, 0.0, 0.0 };
    public double Radius { get; set; } = 1.0;
    public double[] Size { get; set; } = new double[] { 1.0, 1.0, 1.0 }; 
    public double[] Normal { get; set; } = new double[] { 0.0, 1.0, 0.0 }; 
    public string Material { get; set; } = "Default";
    public double RotationAngle { get; set; } = 0.0; 
}

public class Light
{
    public string Type { get; set; } = "Point";
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
    public int SamplesPerPixel { get; set; } = 1;
    public double MinimalPerformance { get; set; } = 0.0;
    public string AntiAliasing { get; set; } = "None";
    public string RayTracer { get; set; } = "Basic";
}

public class SceneConfig : Options
{
    public List<Material> Materials { get; set; } = new List<Material> { new Material() };
    public List<SceneObject> ObjectsInScene { get; set; } = new List<SceneObject> { new SceneObject() };
    public List<Light> Lights { get; set; } = new List<Light> { new Light() };
    public CameraSettings CameraSettings { get; set; } = new CameraSettings();
    public AlgorithmSettings AlgorithmSettings { get; set; } = new AlgorithmSettings();
}
