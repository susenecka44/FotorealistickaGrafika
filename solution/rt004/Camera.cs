using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

/// <summary>
/// Ray class that represents a ray in 3D space
/// </summary>
public class Ray
{
    public Vector3d Origin { get; set; }
    public Vector3d Direction { get; set; }

    public Ray(Vector3d origin, Vector3d direction)
    {
        Origin = origin;
        Direction = direction;  
    }
    public Vector3d PointAtParameter(double t)
    {
        return Origin + t * Direction;
    }
}

/// <summary>
/// Interface for all cameras
/// </summary>
public interface ICamera
{
    public Ray GetRay(double u, double v);
}


/// <summary>
/// Perspective camera class using a perspective projection
/// </summary>
public class PerspectiveCamera : ICamera
{
    public Vector3d LowerLeftCorner { get; set; }
    public Vector3d Horizontal { get; set; }
    public Vector3d Vertical { get; set; }
    public Vector3d Direction { get; set; }
    public double FovDegrees { get; set; }
    public Vector3d Origin { get; set; }


    public PerspectiveCamera(Vector3d cameraPosition, int imageWidth, int imageHeight, double fovDegrees, Vector3d direction)
    {
        Origin = cameraPosition;
        Direction = Vector3d.Normalize(direction);
        FovDegrees = fovDegrees;

        double theta = fovDegrees * (double)Math.PI / 180;
        double h = (double)Math.Tan(theta / 2);
        double viewportHeight = 2.0f * h;
        double aspectRatio = (double)imageWidth / imageHeight;
        double viewportWidth = aspectRatio * viewportHeight;

        Vector3d w = Vector3d.Normalize(-Direction); // Assuming looking in the direction of the negative z-axis
        Vector3d up = new Vector3d(0, -1, 0); 
        Vector3d u = Vector3d.Normalize(Vector3d.Cross(up, w));
        Vector3d v = Vector3d.Cross(w, u);

        Horizontal = viewportWidth * u;
        Vertical = viewportHeight * v;
        LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - w;
    }

    public Ray GetRay(double u, double v)
    {
        return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
    }
}


