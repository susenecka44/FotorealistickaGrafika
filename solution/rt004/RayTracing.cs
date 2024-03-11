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
        if (!WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            // If there's no hit, return the background color
            return ComputeBackgroundColor(r.Direction);
        }

        // Local color calculation can be a separate method which calculates the color based on lights, material and normal
        Vector3 localColor = RayColor(r, world, lights);
        Vector3 reflectColor = Vector3.Zero;
        Vector3 refractColor = Vector3.Zero;
        float colorX = localColor.X * (float)rec.Material.kA;
        float colorY = localColor.Y * (float)rec.Material.kA;
        float colorZ = localColor.Z * (float)rec.Material.kA;

        Vector3 finalColor = new Vector3(colorX, colorY, colorZ); // Start with ambient component of the local color

        // Reflection
        if (rec.Material.Reflectivity > 0 && RenderReflections)
        {
            Vector3 reflectDir = Vector3.Reflect(r.Direction, rec.Normal);
            Ray reflectedRay = new Ray(rec.HitPoint + rec.Normal * 0.001f, reflectDir);
            reflectColor = TraceRay(reflectedRay, world, lights, depth - 1);
        }

        // Fresnel effect calculation
        float fresnelEffect = (float)FresnelCalculation(r.Direction, rec.Normal, rec.Material.Refractivity);
        float reflectance = (float)rec.Material.kS * fresnelEffect; // Reflectance based on Fresnel effect and specular component
        float transmittance = (float)rec.Material.kS * (1 - fresnelEffect); // Transmittance based on Fresnel effect and specular component

        // Refraction
        if (transmittance > 0 && RenderRefractions)
        {
            Vector3 refractDir = ComputeRefractDirection(r.Direction, rec.Normal, rec.Material.Refractivity);
            if (refractDir != Vector3.Zero) // No total internal reflection
            {
                Vector3 outwardNormal;
                float niOverNt;
                if (Vector3.Dot(r.Direction, rec.Normal) > 0) // Ray is inside the object
                {
                    outwardNormal = -rec.Normal;
                    niOverNt = rec.Material.Refractivity;
                }
                else // Ray is outside the object
                {
                    outwardNormal = rec.Normal;
                    niOverNt = 1.0f / rec.Material.Refractivity;
                }
                Ray refractedRay = new Ray(rec.HitPoint + outwardNormal * minPerformance, refractDir);
                refractColor = TraceRay(refractedRay, world, lights, depth - 1);
            }
        }

        // Combine the local, reflected, and refracted colors
        finalColor += reflectance * reflectColor + transmittance * refractColor;
        return finalColor;
    }
    public Vector3 RayColor(Ray r, List<IHittable> world, List<LightSource> lights)
    {
        HitRecord rec;
        Vector3 ambientColor = new Vector3(0, 0, 0);
        Vector3 diffuseColor = new Vector3(0, 0, 0);
        Vector3 specularColor = new Vector3(0, 0, 0);

        if (WorldHit(world, r, minPerformance, float.MaxValue, out rec))
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
            else if (RenderReflections && !RenderShadows) {
                color = ambientColor + specularColor;
            }
            else {
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



    /// <summary>
    /// Calculate the Fresnel effect using Schlick's approximation
    /// </summary>
    double FresnelCalculation(Vector3 I, Vector3 N, float ior)
    {
        double cosi = Math.Clamp(Vector3.Dot(I, N), -1, 1);
        double etai = 1, etat = ior;
        if (cosi > 0) {
            double temp = etai; 
            etai = etat; 
            etat = temp; 
        } 
                                                                      
        double sint = etai / etat * Math.Sqrt(Math.Max(0f, 1 - cosi * cosi));
        // Total internal reflection
        if (sint >= 1)
        {
            return 1;
        }
        else
        {
            double cost = Math.Sqrt(Math.Max(0f, 1 - sint * sint));
            cosi = Math.Abs(cosi);
            double Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
            double Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
            return (Rs * Rs + Rp * Rp) / 2;
        }
    }

    private Vector3 ComputeRefractDirection(Vector3 I, Vector3 N, float ior)
    {
        float cosi = -Math.Max(-1, Math.Min(1, Vector3.Dot(I, N)));
        float etai = 1, etat = ior;
        Vector3 n = N;
        if (cosi < 0) { cosi = -cosi; } else { swap(ref etai, ref etat); n = -N; }
        float eta = etai / etat;
        float k = 1 - eta * eta * (1 - cosi * cosi);
        return k < 0 ? Vector3.Zero : eta * I + (eta * cosi - (float)Math.Sqrt(k)) * n;
    }

    private void swap(ref float a, ref float b)
    {
        float temp = a;
        a = b;
        b = temp;
    }

}
