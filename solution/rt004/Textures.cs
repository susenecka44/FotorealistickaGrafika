using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Interface for all textures
/// </summary>
public interface ITexture
{
    Vector3d ColorAt(double u, double v, Vector3d p);
    Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal);
}

/// <summary>
/// Checker texture class that creates a checker pattern
/// using only color mapping (no normal mapping)
/// </summary>
public class CheckerTexture : ITexture
{
    private Vector3d color1;
    private Vector3d color2;
    public double size;

    /// <summary>
    /// Defines a checker texture with two colors and a size
    /// </summary>
    /// <param name="color1"> first color of the pattern</param>
    /// <param name="color2"> second color of the pattern</param>
    /// <param name="size"> biggger size = smaller squares </param>
    public CheckerTexture(Vector3d color1, Vector3d color2, double size)
    {
        this.color1 = color1;
        this.color2 = color2;
        this.size = size;
    }

    public Vector3d ColorAt(double u, double v, Vector3d p)
    {
        double sines = Math.Sin(size * p.X) * Math.Sin(size * p.Y) * Math.Sin(size * p.Z);
        return sines < 0 ? color1 : color2;
    }
    public Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal)
    {
        // Return the input normal unmodified
        return normal;
    }
}

/// <summary>
/// Solid texture class that creates a solid color texture (no color mapping, no normal mapping)
/// </summary>
public class SolidTexture : ITexture
{
    private Vector3d color;

    public SolidTexture(Vector3d color)
    {
        this.color = color;
    }

    public Vector3d ColorAt(double u, double v, Vector3d p)
    {
        return color;
    }
    public Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal)
    {
        // Return the input normal unmodified
        return normal;
    }
}


/// <summary>
/// Creates a wood-like texture ( color mapping and normal mapping usin Perlin noise )
/// </summary>
public class WoodTexture : ITexture
{
    private Vector3d lightColor;
    private Vector3d darkColor;
    public double ringFrequency;
    public double grainFrequency;

    /// <summary>
    /// Wood texture constructor with light and dark color and ring frequency
    /// </summary>
    /// <param name="lightColor"> lighter color of wood </param>
    /// <param name="darkColor"> darker color of wood </param>
    /// <param name="ringFrequency"> frequency of ring generation </param>
    public WoodTexture(Vector3d lightColor, Vector3d darkColor, double ringFrequency)
    {
        this.lightColor = lightColor;
        this.darkColor = darkColor;
        this.ringFrequency = ringFrequency;
        this.grainFrequency = ringFrequency / 4;
    }

    public Vector3d ColorAt(double u, double v, Vector3d p)
    {
        double distance = Math.Sqrt(p.X * p.X + p.Z * p.Z);
        double grain = Math.Sin(distance * ringFrequency + SimpleNoise(p * grainFrequency)) * 0.5 + 0.5;
        return Vector3d.Lerp(darkColor, lightColor, grain);
    }

    public Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal)
    {
        double epsilon = 0.001;
        Vector3d dpdx = new Vector3d(1, 0, 0);
        Vector3d dpdy = new Vector3d(0, 1, 0);

        double noiseX = SimpleNoise(p + dpdx * epsilon);
        double noiseY = SimpleNoise(p + dpdy * epsilon);

        Vector3d grad = new Vector3d(noiseX - SimpleNoise(p), noiseY - SimpleNoise(p), 0).Normalized();

        // Perturb normal based on the gradient
        Vector3d perturbedNormal = (normal + grad * 0.1).Normalized();
        return perturbedNormal;
    }

    private double SimpleNoise(Vector3d point)
    {
        double dotProduct = Vector3d.Dot(point, new Vector3d(12.9898, 78.233, 54.53));
        double sin = Math.Sin(dotProduct) * 43758.5453;
        return sin - Math.Floor(sin);
    }

    private static Vector3d Lerp(Vector3d a, Vector3d b, double t)
    {
        return a + (b - a) * t;
    }
    
}

