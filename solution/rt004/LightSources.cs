using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using OpenTK.Mathematics;
using System.Threading.Tasks;

/// <summary>
/// Light source abstract class for all light sources
/// </summary>
public abstract class LightSource
{
    public Vector3d Position { get; set; }
    public Vector3d Color { get; set; }

}

/// <summary>
/// Ambient light source class
/// -> light source that illuminates all objects in the scene equally
/// </summary>
public class AmbientLight : LightSource
{
    public AmbientLight(Vector3d color, double intensity) {
        Position = new Vector3d(0, 0, 0);
        double cX = color.X * intensity;
        double cY = color.Y * intensity;
        double cZ = color.Z * intensity;
        Color = new Vector3d(cX, cY, cZ);
    }
}

/// <summary>
/// Directional light source class
/// -> light source that illuminates objects from a specific direction
/// </summary>
public class PointLight : LightSource
{
    public PointLight(Vector3d position, Vector3d color, double intensity)
    {
        Position = position;
        double cX = color.X * intensity;
        double cY = color.Y * intensity;
        double cZ = color.Z * intensity;
        Color = new Vector3d(cX, cY, cZ);
    }
}
