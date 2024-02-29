using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

class Raytracing
{
    // Without LightSources
    /*
    public Vector3 RayColor(Ray r, List<IHittable> world)
    {
        HitRecord rec;

        // Check if we hit anything in the world
        if (WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            // Normal-based coloring: Normalize the normal vector to be between 0 and 1
            return 0.5f * (rec.Normal + new Vector3(1, 1, 1));
        }

        // If no hit, return a background gradient (could be eddited to only white)
        // return new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 unitDirection = Vector3.Normalize(r.Direction);
        float t = 0.5f * (unitDirection.Y + 1.0f);
        // Linearly interpolate between white (up) and blue (down) based on the y coordinate
        return (1.0f - t) * new Vector3(1.0f, 1.0f, 1.0f) + t * new Vector3(0.5f, 0.7f, 1.0f);
    }
    */

    public Vector3 RayColor(Ray r, List<IHittable> world, PointLight light)
    {
        HitRecord rec;

        if (WorldHit(world, r, 0.001f, float.MaxValue, out rec))
        {
            Ray shadowRay = new Ray(rec.P + rec.Normal * 0.001f, light.Position - rec.P);
            if (WorldHit(world, shadowRay, 0.001f, 1.0f, out _)) // Check for shadow
            {
                return new Vector3(0, 0, 0); // In shadow
            }

            float brightness = Math.Max(0, Vector3.Dot(Vector3.Normalize(light.Position - rec.P), rec.Normal));
            Vector3 lightColor = brightness * light.Color;
            return 0.5f * (rec.Normal + new Vector3(1, 1, 1)) + lightColor;
        }

        // Background gradient
        Vector3 unitDirection = Vector3.Normalize(r.Direction);
        float t = 0.5f * (unitDirection.Y + 1.0f);
        return (1.0f - t) * new Vector3(1.0f, 1.0f, 1.0f) + t * new Vector3(0.5f, 0.7f, 1.0f);
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