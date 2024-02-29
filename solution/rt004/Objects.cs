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
    public Vector3 P;
    public Vector3 Normal;
    public float T;
    public bool FrontFace;

    public void SetFaceNormal(Ray r, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
        Normal = FrontFace ? outwardNormal : -outwardNormal;
    }
}

public class Sphere : IHittable
{
    public Vector3 Center { get; }
    public float Radius { get; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3 oc = r.Origin - Center;
        float a = r.Direction.LengthSquared();
        float half_b = Vector3.Dot(oc, r.Direction);
        float c = oc.LengthSquared() - Radius * Radius;
        float discriminant = half_b * half_b - a * c;

        if (discriminant > 0)
        {
            float root = MathF.Sqrt(discriminant);
            float temp = (-half_b - root) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.P = r.PointAtParameter(rec.T);
                Vector3 outwardNormal = (rec.P - Center) / Radius;
                rec.SetFaceNormal(r, outwardNormal);
                return true;
            }
            temp = (-half_b + root) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.P = r.PointAtParameter(rec.T);
                Vector3 outwardNormal = (rec.P - Center) / Radius;
                rec.SetFaceNormal(r, outwardNormal);
                return true;
            }
        }
        return false;
    }
}

public class Cube : IHittable
{
    public Vector3 Min, Max;

    public Cube(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    private bool HitSlab(float min, float max, float rayOrigin, float rayDirection, ref float tMin, ref float tMax)
    {
        float invD = 1.0f / rayDirection;
        float t0 = (min - rayOrigin) * invD;
        float t1 = (max - rayOrigin) * invD;
        if (invD < 0.0f)
        {
            (t0, t1) = (t1, t0);
        }
        tMin = t0 > tMin ? t0 : tMin;
        tMax = t1 < tMax ? t1 : tMax;
        return tMax >= tMin;
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();

        if (!HitSlab(Min.X, Max.X, r.Origin.X, r.Direction.X, ref tMin, ref tMax) ||
            !HitSlab(Min.Y, Max.Y, r.Origin.Y, r.Direction.Y, ref tMin, ref tMax) ||
            !HitSlab(Min.Z, Max.Z, r.Origin.Z, r.Direction.Z, ref tMin, ref tMax))
        {
            return false;
        }

        rec.T = tMin;
        rec.P = r.PointAtParameter(rec.T);
        return true;
    }
}



