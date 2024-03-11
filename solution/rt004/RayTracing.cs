using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

public class Raytracer
{
    private Vector3 backgroundColor;
    private bool RenderShadows;
    private bool RenderReflections;
    private bool RenderRefractions;
    private int maxDepth;
    private float minPerformance;

    public Raytracer(Vector3 backgroundColor, int maxDepth, float minPerformance, bool shadows, bool reflections, bool refractions)
    {
        this.backgroundColor = backgroundColor;
        this.maxDepth = maxDepth;
        this.minPerformance = minPerformance;
        this.RenderReflections = reflections;
        this.RenderShadows = shadows;
        this.RenderRefractions = refractions;
    }
    public Vector3 TraceRay(Ray r, List<IHittable> world, List<LightSource> lights, int depth)
    {
        if (depth <= 0)
            return Vector3.Zero;

        HitRecord rec;
        if (WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            Vector3 reflectedColor = Vector3.Zero;
            Vector3 refractedColor = Vector3.Zero;
            Vector3 color = RayColor(r, world, lights); // Compute local color

            // Reflections
            if (RenderReflections && rec.Material.Reflectivity > 0)
            {
                Vector3 reflectDir = Vector3.Reflect(r.Direction, rec.Normal);
                Ray reflectRay = new Ray(rec.HitPoint + rec.Normal * 0.001f, reflectDir);
                reflectedColor = TraceRay(reflectRay, world, lights, depth - 1);
                // Combine reflection with local color
                color += rec.Material.Reflectivity * reflectedColor;
            }

            // Refractions
            if (RenderRefractions && rec.Material.Refractivity > 0)
            {
                Vector3 refractDir = Calculations.ComputeRefractedDirection(r.Direction, rec.Normal, rec.Material.Refractivity);
                if (refractDir != Vector3.Zero) // Refraction occurred
                {
                    Ray refractRay = new Ray(rec.HitPoint - rec.Normal * 0.001f, refractDir);
                    refractedColor = TraceRay(refractRay, world, lights, depth - 1);
                }
                color += rec.Material.Refractivity * refractedColor;
            }

            return color;
        }
        else
        {
            return ComputeBackgroundColor(r.Direction);
        }
    }

    public Vector3 RayColor(Ray r, List<IHittable> world, List<LightSource> lights)
    {
        HitRecord rec;
        Vector3 ambientColor = new Vector3(0, 0, 0);
        Vector3 diffuseColor = new Vector3(0, 0, 0);
        Vector3 specularColor = new Vector3(0, 0, 0);

        if (WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            foreach (var light in lights)
            {
                Vector3 lightDir = Vector3.Normalize(light.Position - rec.HitPoint);
                float nDotL = Math.Max(Vector3.Dot(rec.Normal, lightDir), 0.0f);

                if (light is AmbientLight)
                {
                    ambientColor += new Vector3(rec.Material.Color[0], rec.Material.Color[1], rec.Material.Color[2]) * (float)rec.Material.kA * light.Color;
                }
                else if (light is PointLight && nDotL > 0)
                {
                    // Check for shadow
                    Vector3 toLight = light.Position - rec.HitPoint;
                    float distanceToLight = toLight.Length();
                    Ray shadowRay = new Ray(rec.HitPoint + rec.Normal * 0.001f, Vector3.Normalize(toLight));
                    if (!WorldHit(world, shadowRay, 0.001f, distanceToLight, out _))
                    {
                        // Diffuse
                        diffuseColor += nDotL * new Vector3(rec.Material.Color[0], rec.Material.Color[1], rec.Material.Color[2]) * (float)rec.Material.kD * light.Color;

                        // Specular
                        Vector3 viewDir = Vector3.Normalize(-r.Direction);
                        Vector3 reflectDir = Vector3.Reflect(-lightDir, rec.Normal);
                        float spec = MathF.Pow(Math.Max(Vector3.Dot(viewDir, reflectDir), 0.0f), (float)rec.Material.HighLight);
                        specularColor += spec * (float)rec.Material.kS * light.Color;
                    }
                }
            }
            Vector3 color;
            if (RenderShadows)
            {
                // Combine all components
                color = ambientColor + diffuseColor + specularColor;
            }
            else if (RenderReflections && !RenderShadows)
            {
                color = ambientColor + specularColor;
            }
            else
            {
                color = ambientColor;
            }
            return color;
        }
        else
        {
            // Background gradient
            return ComputeBackgroundColor(r.Direction);
        }
    }

    bool WorldHit(List<IHittable> world, Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        bool hitAnything = false;
        float closestSoFar = tMax;

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

    private Vector3 ComputeBackgroundColor(Vector3 direction)
    {
        Vector3 unitDirection = Vector3.Normalize(direction);
        float t = 0.5f * (unitDirection.Y + 1.0f);
        return (1.0f - t) * backgroundColor + t * new Vector3(0.5f, 0.7f, 1.0f);
    }
}

public class Calculations
{
    private float CalculateFresnel(Ray r, HitRecord rec, float refractivity)
    {
        float cosi = Math.Clamp(Vector3.Dot(-r.Direction, rec.Normal), -1, 1);
        float etai = 1, etat = refractivity;
        if (cosi > 0) { var temp = etai; etai = etat; etat = temp; }
        float sint = etai / etat * (float)Math.Sqrt(Math.Max(0f, 1 - cosi * cosi));
        if (sint >= 1) return 1; // Total internal reflection
        float cost = (float)Math.Sqrt(Math.Max(0f, 1 - sint * sint));
        cosi = Math.Abs(cosi);
        float Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
        float Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
        return (Rs * Rs + Rp * Rp) / 2;
    }

    public static Vector3 ComputeRefractedDirection(Vector3 direction, Vector3 normal, float refractivity)
    {
        float cosi = Math.Clamp(Vector3.Dot(-direction, normal), -1, 1);
        float etai = 1, etat = refractivity;
        Vector3 n = normal;
        if (cosi < 0) { cosi = -cosi; } else { etai = refractivity; etat = 1; n = -normal; }
        float eta = etai / etat;
        float k = 1 - eta * eta * (1 - cosi * cosi);
        if (k < 0)
        {
            return Vector3.Zero;
        }
        else
        {
            float x = (float)(eta * cosi - Math.Sqrt(k)) * n.X;
            float y = (float)(eta * cosi - Math.Sqrt(k)) * n.Y;
            float z = (float)(eta * cosi - Math.Sqrt(k)) * n.Z;

            return eta * direction + new Vector3(x,y,z);
        }
    }

}