using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


public interface IHittable
{
    bool Hit(Ray r, double tMin, double tMax, out HitRecord rec);
}

public struct HitRecord
{
    public Vector3d HitPoint;
    public Vector3d Normal;
    public double T;
    public bool FrontFace;
    public ObjectMaterial Material;

    public void SetFaceNormal(Ray r, Vector3d outwardNormal)
    {
        FrontFace = Vector3d.Dot(r.Direction, outwardNormal) < 0;

        if (FrontFace){
            Normal = outwardNormal;
        } else {
            Normal = -outwardNormal;
        }
    }
}

public class Sphere : IHittable
{
    public Vector3d Center { get; set; }
    public double Radius { get; set; }
    public ObjectMaterial Material { get; set; }

    public Sphere(Vector3d center, double radius, ObjectMaterial material)
    {
        Center = center;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3d OriginToCenter = ray.Origin - Center;
        // at^2 + bt + c = 0
        // This coefficient represents the dot product of the ray's direction vector with itself
        double a = ray.Direction.LengthSquared();
        double half_b = Vector3d.Dot(OriginToCenter, ray.Direction);
        double c = OriginToCenter.LengthSquared() - Radius * Radius;
        double discriminant = half_b * half_b - a * c;

        if (discriminant > 0)
        {
            double root = Math.Sqrt(discriminant);
            double temp = (-half_b - root) / a;
            rec.Material = Material;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.HitPoint = ray.PointAtParameter(rec.T);
                Vector3d outwardNormal = (rec.HitPoint - Center) / Radius;
                rec.SetFaceNormal(ray, outwardNormal);
                return true;
            }
            temp = (-half_b + root) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.T = temp;
                rec.HitPoint = ray.PointAtParameter(rec.T);
                Vector3d outwardNormal = (rec.HitPoint - Center) / Radius;
                rec.SetFaceNormal(ray, outwardNormal);
                return true;
            }
        }
        return false;
    }
}

public class Plane : IHittable
{
    public Vector3d Point { get; set; } // A point on the plane
    public Vector3d Normal { get; set; } // The normal vector to the plane
    public ObjectMaterial Material { get; set; } // The material of the plane

    public Plane(Vector3d point, Vector3d normal, ObjectMaterial material)
    {
        Point = point;
        Normal = Vector3d.Normalize(normal); // Ensure the normal is normalized
        Material = material;
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        double denominator = Vector3d.Dot(Normal, r.Direction);

        if (Math.Abs(denominator) > 1e-6) // Check if the ray is parallel to the plane
        {
            Vector3d originToPoint = Point - r.Origin;
            double t = Vector3d.Dot(originToPoint, Normal) / denominator;

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
    public Vector3d Center { get; private set; }
    public Vector3d Size { get; private set; }
    private Vector3d Min;
    private Vector3d Max;
    public ObjectMaterial Material { get; set; }
    public double RotationY { get; set; } // Rotation around the Y axis in degrees
    private List<Plane> planes = new List<Plane>();

    public Cube(Vector3d center, Vector3d size, ObjectMaterial material, double rotationY)
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
            new Plane(Max, Vector3d.UnitY, Material), // Top
            new Plane(Min, -Vector3d.UnitY, Material), // Bottom
            new Plane(Min, -Vector3d.UnitX, Material), // Left
            new Plane(Max, Vector3d.UnitX, Material), // Right
            new Plane(Min, -Vector3d.UnitZ, Material), // Front
            new Plane(Max, Vector3d.UnitZ, Material) // Back
        };
    }

    public void SetPosition(Vector3d newCenter)
    {
        Center = newCenter;
        UpdateMinMax();
        InitializePlanes(); // Re-initialize planes as Min and Max have changed
    }

    private Vector3d RotateVectorY(Vector3d v, double degrees)
    {
        double radians = degrees * (Math.PI / 180.0f);
        double cosTheta = Math.Cos(radians);
        double sinTheta = Math.Sin(radians);
        return new Vector3d(
            v.X * cosTheta + v.Z * sinTheta,
            v.Y,
            -v.X * sinTheta + v.Z * cosTheta
        );
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        // Transform the ray to the cube's local space (including rotation)
        Vector3d rotatedOrigin = RotateVectorY(r.Origin - Center, -RotationY) + Center;
        Vector3d rotatedDirection = RotateVectorY(r.Direction, -RotationY);
        Ray rotatedRay = new Ray(rotatedOrigin, rotatedDirection);

        // Perform the intersection test with the transformed ray
        rec = new HitRecord();
        Vector3d invD = new Vector3d(1.0f / rotatedRay.Direction.X, 1.0f / rotatedRay.Direction.Y, 1.0f / rotatedRay.Direction.Z);
        Vector3d t0s = (Min - rotatedRay.Origin) * invD;
        Vector3d t1s = (Max - rotatedRay.Origin) * invD;

        Vector3d tMinVec = Vector3dExtensions.Min(t0s, t1s);
        Vector3d tMaxVec = Vector3dExtensions.Max(t0s, t1s);

        double tMinSlab = Math.Max(tMinVec.X, Math.Max(tMinVec.Y, tMinVec.Z));
        double tMaxSlab = Math.Min(tMaxVec.X, Math.Min(tMaxVec.Y, tMaxVec.Z));

        if (tMaxSlab < 0 || tMinSlab > tMaxSlab)
        {
            return false;
        }

        bool isEntering = tMinSlab > tMin;
        rec.T = isEntering ? tMinSlab : tMaxSlab;
        rec.HitPoint = rotatedRay.PointAtParameter(rec.T);
        // Calculate normal based on intersection
        rec.Normal = Vector3d.Zero; // Initialize the normal
        if (isEntering)
        {
            if (tMinSlab == tMinVec.X) rec.Normal = invD.X < 0 ? Vector3d.UnitX : -Vector3d.UnitX;
            if (tMinSlab == tMinVec.Y) rec.Normal = invD.Y < 0 ? Vector3d.UnitY : -Vector3d.UnitY;
            if (tMinSlab == tMinVec.Z) rec.Normal = invD.Z < 0 ? Vector3d.UnitZ : -Vector3d.UnitZ;
        }
        else
        {
            if (tMaxSlab == tMaxVec.X) rec.Normal = invD.X < 0 ? -Vector3d.UnitX : Vector3d.UnitX;
            if (tMaxSlab == tMaxVec.Y) rec.Normal = invD.Y < 0 ? -Vector3d.UnitY : Vector3d.UnitY;
            if (tMaxSlab == tMaxVec.Z) rec.Normal = invD.Z < 0 ? -Vector3d.UnitZ : Vector3d.UnitZ;
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

public class Cylinder : IHittable
{
    public Vector3d BaseCenter { get; private set; } // = Position
    public Vector3d TopCenter { get; private set; }  
    public double Radius { get; private set; }
    public double Height { get; private set; }
    public ObjectMaterial Material { get; set; }

    public Cylinder(Vector3d baseCenter, double height, double radius, ObjectMaterial material)
    {
        BaseCenter = baseCenter;
        TopCenter = new Vector3d(baseCenter.X, baseCenter.Y + height, baseCenter.Z);
        Height = height;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        bool hasHit = false;
        double closestT = tMax;

        Vector3d axis = TopCenter - BaseCenter;
        Vector3d oc = ray.Origin - BaseCenter;
        double axisDotDirection = Vector3d.Dot(axis, ray.Direction);
        double axisDotOC = Vector3d.Dot(axis, oc);
        double axisLengthSquared = axis.LengthSquared();
        Vector3d perpendicular = ray.Direction - axis * (axisDotDirection / axisLengthSquared);
        Vector3d ocPerpendicular = oc - axis * (axisDotOC / axisLengthSquared);

        double a = perpendicular.LengthSquared();
        double b = 2.0 * Vector3d.Dot(perpendicular, ocPerpendicular);
        double c = ocPerpendicular.LengthSquared() - Radius * Radius;

        double discriminant = b * b - 4 * a * c;
        if (discriminant >= 0)
        {
            double sqrtD = Math.Sqrt(discriminant);
            double t0 = (-b - sqrtD) / (2 * a);
            double t1 = (-b + sqrtD) / (2 * a);

            if (t0 > t1)
            {
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            if (CheckIntersectionAtT(ray, t0, axis, axisDotOC, axisDotDirection, axisLengthSquared, tMin, closestT, ref rec))
            {
                closestT = rec.T;
                hasHit = true;
            }
            if (CheckIntersectionAtT(ray, t1, axis, axisDotOC, axisDotDirection, axisLengthSquared, tMin, closestT, ref rec))
            {
                closestT = rec.T;
                hasHit = true;
            }
        }

        if (CheckCap(ray, BaseCenter, axis, Radius, tMin, ref closestT, out HitRecord capRecBottom))
        {
            rec = capRecBottom;
            hasHit = true;
        }
        if (CheckCap(ray, TopCenter, -axis, Radius, tMin, ref closestT, out HitRecord capRecTop))
        {
            rec = capRecTop;
            hasHit = true;
        }

        return hasHit;
    }

    private bool CheckCap(Ray ray, Vector3d capCenter, Vector3d axis, double radius, double tMin, ref double closestT, out HitRecord rec)
    {
        rec = new HitRecord();
        double t = Vector3d.Dot(capCenter - ray.Origin, axis) / Vector3d.Dot(ray.Direction, axis);
        if (t < tMin || t >= closestT)
        {
            return false;
        }

        Vector3d p = ray.PointAtParameter(t);
        double distance = (p - capCenter).LengthSquared();
        if (distance <= radius * radius)
        {
            closestT = t;
            rec.T = t;
            rec.HitPoint = p;
            rec.Normal = axis.Normalized();
            rec.Material = Material;
            rec.FrontFace = Vector3d.Dot(ray.Direction, axis) < 0;
            return true;
        }
        return false;
    }

    private bool CheckIntersectionAtT(Ray ray, double t, Vector3d axis, double axisDotOC, double axisDotDirection, double axisLengthSquared, double tMin, double tMax, ref HitRecord rec)
    {
        double z = axisDotOC + t * axisDotDirection;
        if (z >= 0 && z <= axisLengthSquared && t >= tMin && t <= tMax)
        {
            rec.T = t;
            rec.HitPoint = ray.PointAtParameter(t);
            Vector3d outwardNormal = (rec.HitPoint - (BaseCenter + axis * (z / axisLengthSquared))) / Radius;
            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = Material;
            return true;
        }
        return false;
    }
}



