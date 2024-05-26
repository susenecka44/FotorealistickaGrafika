using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTK.Mathematics;
using System.Threading.Tasks;

public interface IRayTracer
{
    public Vector3d TraceRay(Ray r, List<IHittable> world, List<LightSource> lights, int depth);
    public Vector3d RayColor(Ray r, List<IHittable> world, List<LightSource> lights);
}

/// <summary>
/// Based on Phongs model-ish
/// </summary>
public class Raytracer : IRayTracer
{
    private Vector3d backgroundColor;
    private bool RenderShadows;
    private bool RenderReflections;
    private bool RenderRefractions;
    private int maxDepth;
    private double minPerformance;

    public Raytracer(Vector3d backgroundColor, int maxDepth, double minPerformance, bool shadows, bool reflections, bool refractions)
    {
        this.backgroundColor = backgroundColor;
        this.maxDepth = maxDepth;
        this.minPerformance = minPerformance;
        this.RenderReflections = reflections;
        this.RenderShadows = shadows;
        this.RenderRefractions = refractions;
    }
    public Vector3d TraceRay(Ray r, List<IHittable> world, List<LightSource> lights, int depth)
    {
        depth = Math.Min(depth, maxDepth);
        if (depth <= 0)
            return Vector3d.Zero;

        HitRecord rec;
        if (WorldHit(world, r, minPerformance, double.MaxValue, out rec))
        {
            Vector3d reflectedColor = Vector3d.Zero;
            Vector3d refractedColor = Vector3d.Zero;
            Vector3d color = RayColor(r, world, lights); // Compute local color


            // Reflections
            if (RenderReflections && rec.Material.Reflectivity > 0)
            {
                Vector3d reflectDir = Vector3dExtensions.Reflect(r.Direction, rec.Normal);
                Ray reflectRay = new Ray(rec.HitPoint + rec.Normal * minPerformance, reflectDir);
                reflectedColor = rec.Material.Reflectivity * TraceRay(reflectRay, world, lights, depth - 1);
                // Combine reflection with local color
                color += reflectedColor;
            }

            // Refractions
            if (RenderRefractions && rec.Material.Refractivity > 0)
            {
                Vector3d refractDir = Calculations.ComputeRefractedDirection(r.Direction, rec.Normal, rec.Material.Refractivity, rec.FrontFace);
                if (refractDir != Vector3d.Zero) // Refraction occurred
                {
                    Ray refractRay = new Ray(rec.HitPoint - rec.Normal * minPerformance, refractDir);
                    refractedColor = rec.Material.Refractivity * TraceRay(refractRay, world, lights, depth - 1);
                }
                color += refractedColor;
               // color += (1 - fresnelEffect) * refractedColor;
            }

            return color;
        }
        else
        {
            return ComputeBackgroundColor(r.Direction);
        }
    }

    public Vector3d RayColor(Ray r, List<IHittable> world, List<LightSource> lights)
    {
        HitRecord rec;
        Vector3d ambientColor = new Vector3d(0, 0, 0);
        Vector3d diffuseColor = new Vector3d(0, 0, 0);
        Vector3d specularColor = new Vector3d(0, 0, 0);

        if (WorldHit(world, r, minPerformance, double.MaxValue, out rec))
        {
            Vector3d materialColor = rec.Material.GetColor(rec.U, rec.V, rec.HitPoint);

            foreach (var light in lights)
            {
                Vector3d lightDir = Vector3d.Normalize(light.Position - rec.HitPoint);
                double nDotL = Math.Max(Vector3d.Dot(rec.Normal, lightDir), 0.0f);

                if (light is AmbientLight)
                {
                    ambientColor += materialColor * rec.Material.kA * light.Color;
                }
                else if (light is PointLight && nDotL > 0)
                {
                    // Check for shadow
                    Vector3d toLight = light.Position - rec.HitPoint;
                    double distanceToLight = toLight.Length();
                    Ray shadowRay = new Ray(rec.HitPoint + rec.Normal * 0.001f, Vector3d.Normalize(toLight));
                    if (!WorldHit(world, shadowRay, 0.001f, distanceToLight, out _))
                    {
                        // Diffuse
                        diffuseColor += nDotL * materialColor * rec.Material.kD * light.Color;

                        // Specular
                        Vector3d viewDir = Vector3d.Normalize(-r.Direction);
                        Vector3d reflectDir = Vector3dExtensions.Reflect(-lightDir, rec.Normal);
                        double spec = Math.Pow(Math.Max(Vector3d.Dot(viewDir, reflectDir), 0.0f), rec.Material.HighLight);
                        specularColor += spec * rec.Material.kS * light.Color;
                    }
                }
            }

            Vector3d color = new Vector3d(0, 0, 0);
            if (RenderShadows)
            {
                // Combine all components
                color += ambientColor + diffuseColor + specularColor;
            }
            else if (RenderReflections && !RenderShadows)
            {
                color += ambientColor + specularColor;
            }
            else
            {
                color += ambientColor;
            }
            // aply texture:
            return color;
        }
        else
        {
            // Background gradient
            return ComputeBackgroundColor(r.Direction);
        }
    }

    bool WorldHit(List<IHittable> world, Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        bool hitAnything = false;
        double closestSoFar = tMax;

        foreach (var o in world)
        {
            if (o.Hit(r, tMin, closestSoFar, out HitRecord tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.T;
                rec = tempRec;
            }
        }
        return hitAnything;
    }

    private Vector3d ComputeBackgroundColor(Vector3d direction)
    {
        Vector3d unitDirection = Vector3d.Normalize(direction);
        double t = 0.5f * (unitDirection.Y + 1.0f);
        return (1.0f - t) * backgroundColor + t * new Vector3d(0.5f, 0.7f, 1.0f);
    }
}

// calculatios for refractions
public class Calculations
{
    public static Vector3d ComputeRefractedDirection(Vector3d direction, Vector3d normal, double refractivity, bool isOutside)
    {
        double n12 = isOutside ? 1 / refractivity : refractivity;

        var cosi = Vector3d.Dot(normal, direction);
        Vector3d refractedDirection =
            Vector3d.Multiply((direction - normal * cosi), n12) -
            normal * Math.Sqrt(1 - n12 * n12 * (1 - cosi * cosi));

        return refractedDirection;
    }
}