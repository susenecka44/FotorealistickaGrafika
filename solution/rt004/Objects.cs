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

public class Cube : IHittable
{
    public Vector3 Center { get; private set; }
    public Vector3 Size { get; private set; }
    private Vector3 Min;
    private Vector3 Max;
    public ObjectMaterial Material { get; set; }
    public float RotationY { get; set; } // Rotation around the Y axis in degrees
    private List<Plane> planes;

    public Cube(Vector3 center, Vector3 size, ObjectMaterial material, float rotationY)
    {
        Center = center;
        Size = size;
        Material = material;
        RotationY = rotationY;
        
        UpdateMinMax();
        InitializePlanes();
    }

    private void UpdateMinMax()
    {
        Min = Center - Size / 2;
        Max = Center + Size / 2;
    }

    private void InitializePlanes()
    {
        planes = new List<Plane>
        {
            new Plane(Max, Vector3.UnitY, Material), // Top
            new Plane(Min, -Vector3.UnitY, Material), // Bottom
            new Plane(Min, -Vector3.UnitX, Material), // Left
            new Plane(Max, Vector3.UnitX, Material), // Right
            new Plane(Min, -Vector3.UnitZ, Material), // Front
            new Plane(Max, Vector3.UnitZ, Material) // Back
        };
    }

    public void SetPosition(Vector3 newCenter)
    {
        Center = newCenter;
        UpdateMinMax();
        InitializePlanes(); // Re-initialize planes as Min and Max have changed
    }

    private Vector3 RotateVectorY(Vector3 v, float degrees)
    {
        float radians = degrees * (MathF.PI / 180.0f);
        float cosTheta = MathF.Cos(radians);
        float sinTheta = MathF.Sin(radians);
        return new Vector3(
            v.X * cosTheta + v.Z * sinTheta,
            v.Y,
            -v.X * sinTheta + v.Z * cosTheta
        );
    }

    public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        // Transform the ray to the cube's local space (including rotation)
        Vector3 rotatedOrigin = RotateVectorY(r.Origin - Center, -RotationY) + Center;
        Vector3 rotatedDirection = RotateVectorY(r.Direction, -RotationY);
        Ray rotatedRay = new Ray(rotatedOrigin, rotatedDirection);

        // Perform the intersection test with the transformed ray
        rec = new HitRecord();
        Vector3 invD = new Vector3(1.0f / rotatedRay.Direction.X, 1.0f / rotatedRay.Direction.Y, 1.0f / rotatedRay.Direction.Z);
        Vector3 t0s = (Min - rotatedRay.Origin) * invD;
        Vector3 t1s = (Max - rotatedRay.Origin) * invD;

        Vector3 tMinVec = Vector3.Min(t0s, t1s);
        Vector3 tMaxVec = Vector3.Max(t0s, t1s);

        float tMinSlab = Math.Max(tMinVec.X, Math.Max(tMinVec.Y, tMinVec.Z));
        float tMaxSlab = Math.Min(tMaxVec.X, Math.Min(tMaxVec.Y, tMaxVec.Z));

        if (tMaxSlab < 0 || tMinSlab > tMaxSlab)
        {
            return false;
        }

        bool isEntering = tMinSlab > tMin;
        rec.T = isEntering ? tMinSlab : tMaxSlab;
        rec.HitPoint = rotatedRay.PointAtParameter(rec.T);
        // Calculate normal based on intersection
        rec.Normal = Vector3.Zero; // Initialize the normal
        if (isEntering)
        {
            if (tMinSlab == tMinVec.X) rec.Normal = invD.X < 0 ? Vector3.UnitX : -Vector3.UnitX;
            if (tMinSlab == tMinVec.Y) rec.Normal = invD.Y < 0 ? Vector3.UnitY : -Vector3.UnitY;
            if (tMinSlab == tMinVec.Z) rec.Normal = invD.Z < 0 ? Vector3.UnitZ : -Vector3.UnitZ;
        }
        else
        {
            if (tMaxSlab == tMaxVec.X) rec.Normal = invD.X < 0 ? -Vector3.UnitX : Vector3.UnitX;
            if (tMaxSlab == tMaxVec.Y) rec.Normal = invD.Y < 0 ? -Vector3.UnitY : Vector3.UnitY;
            if (tMaxSlab == tMaxVec.Z) rec.Normal = invD.Z < 0 ? -Vector3.UnitZ : Vector3.UnitZ;
        }

        // Transform the normal back to the world space (considering rotation)
        rec.Normal = RotateVectorY(rec.Normal, RotationY);
        rec.Material = Material;
        rec.SetFaceNormal(rotatedRay, rec.Normal); // Ensure normal is correctly oriented with respect to the rotated ray

        // Adjust hit point to world space
        rec.HitPoint = RotateVectorY(rec.HitPoint - Center, RotationY) + Center;

        return true;
    }
}

