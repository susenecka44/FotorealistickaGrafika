using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

public class Raycasting
{
    private Vector3 backgroundColor;

    public Raycasting(Vector3 backgroundColor) {  this.backgroundColor = backgroundColor; }
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

            // Combine all components
            Vector3 color = ambientColor + diffuseColor + specularColor;
            return color;
        }
        else
        {
            // Background gradient
            Vector3 unitDirection = Vector3.Normalize(r.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - t) * backgroundColor + t * new Vector3(0.5f, 0.7f, 1.0f);
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
}
