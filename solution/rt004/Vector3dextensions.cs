using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

/// <summary>
/// Vector3dExtensions class that contains extension methods for the Vector3d class based on methods from the Vector3 class in OpenTK.Mathematics
/// </summary>
// previous code based on Vector3 based on floats - these are extensions for the methods that arent avaiable in OpenTK.Mathematics, but are in Vecotr3
public static class Vector3dExtensions
{
    // calculates the squared length of the vector.
    public static double LengthSquared(this Vector3d vector)
    {
        return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }

    // returns a vector that is made up of the smallest components of two vectors
    public static Vector3d Min(Vector3d a, Vector3d b)
    {
        return new Vector3d(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    }

    // returns a vector that is made up of the largest components of two vectors
    public static Vector3d Max(Vector3d a, Vector3d b)
    {
        return new Vector3d(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    }
    public static Vector3d Reflect(Vector3d vector, Vector3d normal)
    {
        // reflection formula: R = V - 2 * (V · N) * N
        // V = incoming vector, N = normal, R = reflected vector.
        double dotProduct = Vector3d.Dot(vector, normal);
        return vector - 2 * dotProduct * normal;
    }

    // returns euclidean length of the vector
    public static double Length(this Vector3d vector)
    {
        return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
    }

    // rotate a vector according to the matrix
    public static Vector3d Transform(this Vector3d vector, Matrix3d matrix)
    {
        return new Vector3d(
            vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31,
            vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32,
            vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33
        );
    }
}
