using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public interface IHittable
{
    bool Hit(Ray r, float tMin, float tMax, out HitRecord rec);
}

public struct HitRecord
{
    public Vector3 HitPoint;
    public Vector3 Normal;
    public float T;
    public bool FrontFace;
    public ObjectMaterial Material;

    public void SetFaceNormal(Ray r, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;

        if (FrontFace){
            Normal = outwardNormal;
        } else {
            Normal = -outwardNormal;
        }
    }
}

// TODO fix
public class Sphere : IHittable
{
    public Vector3 Center { get; set; }
    public float Radius { get; set; }
    public ObjectMaterial Material { get; set; }

    public Sphere(Vector3 center, float radius, ObjectMaterial material)
    {
        Center = center;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray ray, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3 OriginToCenter = ray.Origin - Center;
        // at^2 + bt + c = 0
        // This coefficient represents the dot product of the ray's direction vector with itself
        float a = ray.Direction.LengthSquared();
        float half_b = Vector3.Dot(OriginToCenter, ray.Direction);
        float c = OriginToCenter.LengthSquared() - Radius * Radius;
        float discriminant = half_b * half_b - a * c;

        if (discriminant > 0)
        {
            float root = MathF.Sqrt(discriminant);
            float temp = (-half_b - root) / a;
            rec.Material = Material;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.HitPoint = ray.PointAtParameter(rec.T);
                Vector3 outwardNormal = (rec.HitPoint - Center) / Radius;
                rec.SetFaceNormal(ray, outwardNormal);
                return true;
            }
            temp = (-half_b + root) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.HitPoint = ray.PointAtParameter(rec.T);
                Vector3 outwardNormal = (rec.HitPoint - Center) / Radius;
                rec.SetFaceNormal(ray, outwardNormal);
                return true;
            }
        }
        return false;
    }
}

public class Plane : IHittable
{
    public Vector3 Point { get; set; } // A point on the plane
    public Vector3 Normal { get; set; } // The normal vector to the plane
    public ObjectMaterial Material { get; set; } // The material of the plane

    public Plane(Vector3 point, Vector3 normal, ObjectMaterial material)
    {
        Point = point;
        Normal = Vector3.Normalize(normal); // Ensure the normal is normalized
        Material = material;
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float denominator = Vector3.Dot(Normal, r.Direction);

        if (Math.Abs(denominator) > 1e-6) // Check if the ray is parallel to the plane
        {
            Vector3 originToPoint = Point - r.Origin;
            float t = Vector3.Dot(originToPoint, Normal) / denominator;

            if (t >= tMin && t <= tMax) // Check if the t value is within bounds
            {
                rec.T = t;
                rec.HitPoint = r.Origin + t * r.Direction;
                rec.SetFaceNormal(r, Normal); // Set normal based on the ray direction
                rec.Material = Material;
                return true;
            }
        }

        return false;
    }
}

