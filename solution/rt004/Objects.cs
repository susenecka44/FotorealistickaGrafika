using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

/// <summary>
/// Interface for all objects that can be hit by a ray
/// </summary>
public interface IHittable
{
    /// <summary>
    /// Hit function that checks if a ray hits the current object based on mathematic calculations
    /// </summary>
    /// <param name="r"> ray with which it is supposed to intersect </param>
    /// <param name="rec"> the out HitRecord class </param>
    /// <returns> Hit Record </returns>
    bool Hit(Ray r, double tMin, double tMax, out HitRecord rec);
}


/// <summary>
/// Hit record struct that stores information about a hit point of a object in scene needed by the raytracer
/// </summary>
public struct HitRecord
{
    public Vector3d HitPoint;
    public Vector3d Normal;
    public double U, V; // Texture coordinates
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

/// <summary>
/// Hitable Sphere object
/// </summary>
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
                // Texture coordinates
                rec.U = 0.5 + Math.Atan2(outwardNormal.Z, outwardNormal.X) / (2 * Math.PI);
                rec.V = 0.5 - Math.Asin(outwardNormal.Y) / Math.PI;

                outwardNormal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, outwardNormal);

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

/// <summary>
/// Hitable Plane object
/// </summary>
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

                rec.U = Vector3d.Dot(rec.HitPoint, Vector3d.UnitX);
                rec.V = Vector3d.Dot(rec.HitPoint, Vector3d.UnitZ);
                rec.Material = Material;

                Normal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, Normal);

                rec.SetFaceNormal(r, Normal); // Set normal based on the ray direction
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// Hitable Cube object
/// </summary>
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
        rec.U = (rec.HitPoint.X - Min.X) / (Max.X - Min.X);
        rec.V = (rec.HitPoint.Y - Min.Y) / (Max.Y - Min.Y);
        rec.Normal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, rec.Normal);

        rec.SetFaceNormal(rotatedRay, rec.Normal); // Ensure normal is correctly oriented with respect to the rotated ray

        // Adjust hit point to world space
        rec.HitPoint = RotateVectorY(rec.HitPoint - Center, RotationY) + Center;

        return true;
    }
}

/// <summary>
/// Hitable Cylinder object
/// </summary>
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


            // Calculate U, V texture coordinates
            rec.U = (Math.Atan2(outwardNormal.Z, outwardNormal.X) / (2 * Math.PI) + 0.5);
            rec.V = Height/2;

            // Apply normal map if any
            rec.Normal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, outwardNormal);

            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = Material;
            return true;
        }
        return false;
    }
}

/// <summary>
/// Hitable Cone object
/// </summary>
public class Cone : IHittable
{
    public Vector3d Apex { get; set; }
    public double Height { get; set; }
    public double Radius { get; set; }
    public ObjectMaterial Material { get; set; }

    public Cone(Vector3d apex, double height, double radius, ObjectMaterial material)
    {
        // Set the apex to be the base point now, since the cone is flipped.
        Apex = apex;
        Height = height;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3d apexToOrigin = r.Origin - Apex;
        double tanTheta = (Radius / Height);
        double a = Vector3d.Dot(r.Direction, r.Direction) - (1 + tanTheta * tanTheta) * Math.Pow(Vector3d.Dot(r.Direction, Vector3d.UnitY), 2);
        double b = 2 * (Vector3d.Dot(r.Direction, apexToOrigin) - (1 + tanTheta * tanTheta) * Vector3d.Dot(r.Direction, Vector3d.UnitY) * Vector3d.Dot(apexToOrigin, Vector3d.UnitY));
        double c = Vector3d.Dot(apexToOrigin, apexToOrigin) - (1 + tanTheta * tanTheta) * Math.Pow(Vector3d.Dot(apexToOrigin, Vector3d.UnitY), 2);

        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return false;

        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double t1 = (-b - sqrtDiscriminant) / (2 * a);
        double t2 = (-b + sqrtDiscriminant) / (2 * a);
        bool hitSomething = false;

        // Check if either root is a valid hit
        double[] roots = { t1, t2 };
        foreach (var t in roots)
        {
            if (t >= tMin && t <= tMax && IsWithinConeHeight(r, t))
            {
                rec.T = t;
                rec.HitPoint = r.PointAtParameter(t);
                Vector3d outwardNormal = Vector3d.Normalize(Vector3d.Cross(Vector3d.Cross(rec.HitPoint - Apex, Vector3d.UnitY), rec.HitPoint - Apex));
                outwardNormal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, outwardNormal);

                // Calculate U, V texture coordinates
                rec.U = 0.5 + Math.Atan2(rec.HitPoint.Z, rec.HitPoint.X) / (2 * Math.PI);
                rec.V = rec.HitPoint.Y / Height;

                // Apply normal map if any
                rec.Normal = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, outwardNormal);


                rec.SetFaceNormal(r, outwardNormal);
                rec.Material = Material;
                hitSomething = true;
                break;
            }
        }

        // Check for top cap
        Vector3d topCapCenter = Apex + new Vector3d(0, Height, 0);
        double tTopCap = (topCapCenter.Y - r.Origin.Y) / r.Direction.Y;
        Vector3d pTopCap = r.PointAtParameter(tTopCap);
        double radius2TopCap = Vector3d.DistanceSquared(pTopCap, topCapCenter);
        if (radius2TopCap <= Radius * Radius && tTopCap >= tMin && tTopCap <= tMax)
        {
            rec.T = tTopCap;
            rec.HitPoint = pTopCap;
            Vector3d normalTopCap = new Vector3d(0, -1, 0);  // Pointing downwards, because the cone is inverted
            
            // Calculate U, V texture coordinates
            rec.U = 0.5 + Math.Atan2(rec.HitPoint.Z, rec.HitPoint.X) / (2 * Math.PI);
            rec.V = rec.HitPoint.Y / Height;

            // Apply normal map if any
            normalTopCap = Material.Texture.NormalAt(rec.U, rec.V, rec.HitPoint, normalTopCap);

            rec.SetFaceNormal(r, normalTopCap);
            rec.Material = Material;
            hitSomething = true;
        }

        return hitSomething;
    }


    private bool IsWithinConeHeight(Ray r, double t)
    {
        Vector3d point = r.PointAtParameter(t);
        double heightAtPoint = Vector3d.Dot(point - Apex, Vector3d.UnitY);
        return heightAtPoint >= 0 && heightAtPoint <= Height;
    }
}

/// <summary>
/// Calculations needed for the hit calculations
/// </summary>
public static class CalculationsOfFormulasNeeded
{
    public static double[] SolveQuartic(double c0, double c1, double c2, double c3, double c4)
    {
        // Normalize coefficients
        double a = c3 / c4;
        double b = c2 / c4;
        double c = c1 / c4;
        double d = c0 / c4;

        // Solve the cubic resolvent
        double p = -b * b / 12 - c;
        double q = -b * b * b / 108 + b * c / 3 - d / 2;
        double[] u = SolveCubic(1, 0, p, q);
        if (u.Length == 0) return new double[0]; // No real solution for cubic

        double y = -5.0 / 6.0 * b + u[0] - p / (3 * u[0]);

        double w = Math.Sqrt(a * a / 4 - b + y);
        if (double.IsNaN(w))
        {
            return new double[0]; // No real solutions
        }

        double x1 = a / 2 + w;
        double x2 = a / 2 - w;

        // Solve quadratic factors
        double[] firstPair = SolveQuadratic(1, x1, y - w * w);
        double[] secondPair = SolveQuadratic(1, x2, y + w * w);

        // Combine the roots
        var roots = new List<double>();
        roots.AddRange(firstPair);
        roots.AddRange(secondPair);

        return roots.ToArray();
    }

    public static double[] SolveCubic(double a, double b, double c, double d)
    {
        // Normalize coefficients
        double A = b / a;
        double B = c / a;
        double C = d / a;

        // Use the substitution x = y - A/3 to eliminate the quadratic term: y^3 + 3py + 2q = 0
        double sq_A = A * A;
        double p = 1.0 / 3 * (-1.0 / 3 * sq_A + B);
        double q = 1.0 / 2 * (2.0 / 27 * A * sq_A - 1.0 / 3 * A * B + C);

        // Use Cardano's formula
        double cb_p = p * p * p;
        double D = q * q + cb_p;

        if (D >= 0)
        {
            double sqrt_D = Math.Sqrt(D);
            double u = Math.Cbrt(-q + sqrt_D);
            double v = Math.Cbrt(-q - sqrt_D);
            double y = u + v;

            return new double[] { y - A / 3 };
        }
        else
        {
            double phi = 1.0 / 3 * Math.Acos(-q / Math.Sqrt(-cb_p));
            double t = 2 * Math.Sqrt(-p);

            return new double[] {
                t * Math.Cos(phi) - A / 3,
                t * Math.Cos(phi + 2 * Math.PI / 3) - A / 3,
                t * Math.Cos(phi - 2 * Math.PI / 3) - A / 3
            };
        }
    }

    public static double[] SolveQuadratic(double a, double b, double c)
    {
        // Prevent division by a very small number
        if (Math.Abs(a) < 1e-6)
        {
            // In this case, the equation is not quadratic, but linear
            if (Math.Abs(b) < 1e-6)
            {
                return new double[0]; // No solution
            }
            return new double[] { -c / b }; // One root
        }

        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            return new double[0]; // No real solutions
        }
        else if (discriminant == 0)
        {
            return new double[] { -b / (2 * a) }; // One real root
        }
        else
        {
            double sqrtD = Math.Sqrt(discriminant);
            return new double[] { (-b + sqrtD) / (2 * a), (-b - sqrtD) / (2 * a) }; // Two real roots
        }
    }
}

