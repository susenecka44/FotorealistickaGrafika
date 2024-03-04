using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

class Raycasting
{
    public Vector3 RayColor(Ray r, List<IHittable> world, PointLight light)
    {
        HitRecord rec;

        if (WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            Vector3 toLight = light.Position - rec.HitPoint;
            float distanceToLight = toLight.Length();
            Ray shadowRay = new Ray(rec.HitPoint + rec.Normal * 0.001f, Vector3.Normalize(toLight));
            if (WorldHit(world, shadowRay, 0.001f, distanceToLight, out _)) // Check for shadow
            {
                // Calculate shadow intensity based on distance
                float shadowIntensity = Math.Max(0, 1 - distanceToLight / 10.0f); // Adjust the denominator for desired effect
                Vector3 shadowColor = new Vector3(10, 10, 10) * shadowIntensity; // Darken the shadow
                return shadowColor; // Return shadow color with gradient effect
            }

            float brightness = Math.Max(0, Vector3.Dot(Vector3.Normalize(toLight), rec.Normal));
            Vector3 lightColor = brightness * light.Color;
            return 0.5f * (rec.Normal + new Vector3(1, 1, 1)) + lightColor;
        }

        // Background gradient
        Vector3 unitDirection = Vector3.Normalize(r.Direction);
        float t = 0.5f * (unitDirection.Y + 1.0f);
        return (1.0f - t) * new Vector3(255, 55, 200) + t * new Vector3(0.5f, 0.7f, 1.0f);
    }

    // This helper function checks if a ray hits any objects in the world
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
