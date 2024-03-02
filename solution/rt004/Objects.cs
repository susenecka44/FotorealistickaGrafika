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

public class Sphere : IHittable
{
    public Vector3 Center { get; set; }
    public float Radius { get; set; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
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

public class Cube1 : IHittable
{
    public Vector3 Min, Max;

    public Cube1(Vector3 min, Vector3 max)
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
        rec.HitPoint = r.PointAtParameter(rec.T);
        return true;
    }
}

public class Triangle : IHittable
{
    public Vector3 V0, V1, V2;
    public Vector3 Normal;

    public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
        Normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        // Implementation of the Moller-Trumbore intersection algorithm
        rec = new HitRecord();
        Vector3 edge1 = V1 - V0;
        Vector3 edge2 = V2 - V0;
        Vector3 h = Vector3.Cross(r.Direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -float.Epsilon && a < float.Epsilon)
            return false; // This means the ray is parallel to the triangle.

        float f = 1.0f / a;
        Vector3 s = r.Origin - V0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0 || u > 1.0)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(r.Direction, q);

        if (v < 0.0 || u + v > 1.0)
            return false;

        float t = f * Vector3.Dot(edge2, q);

        if (t > tMin && t < tMax)
        {
            rec.T = t;
            rec.HitPoint = r.PointAtParameter(t);
            rec.Normal = Normal;
            rec.SetFaceNormal(r, rec.Normal);
            return true;
        }

        return false;
    }
}

public class Cube : IHittable
{
    private List<IHittable> faces;

    public Cube(Vector3 center, float side)
    {
        faces = new List<IHittable>();
        float halfSide = side / 2;
        // Define vertices for cube faces
        // Add faces as triangles
        // Example for one face (you'll need to repeat this for each face with the correct vertices):
        Vector3 v0 = new Vector3(center.X - halfSide, center.Y - halfSide, center.Z + halfSide);
        Vector3 v1 = new Vector3(center.X + halfSide, center.Y - halfSide, center.Z + halfSide);
        Vector3 v2 = new Vector3(center.X + halfSide, center.Y + halfSide, center.Z + halfSide);
        Vector3 v3 = new Vector3(center.X - halfSide, center.Y + halfSide, center.Z + halfSide);
        // Front face
        faces.Add(new Triangle(v0, v1, v2));
        faces.Add(new Triangle(v0, v2, v3));
        // Repeat for other faces...
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        bool hitAnything = false;
        float closestSoFar = tMax;

        foreach (var face in faces)
        {
            if (face.Hit(r, tMin, closestSoFar, out HitRecord tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.T;
                rec = tempRec;
            }
        }

        return hitAnything;
    }
}


