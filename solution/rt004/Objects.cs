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

