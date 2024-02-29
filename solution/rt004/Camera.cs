using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


public struct Ray
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;  
    }
    public Vector3 PointAtParameter(float t)
    {
        return Origin + t * Direction;
    }
}

public class Camera
{
    public Vector3 Origin { get; set; }
    public Vector3 HigherLeftCorner { get; set; }
    public Vector3 Horizontal {  get; set; }
    public Vector3 Vertical { get; set; }

    public Camera()
    {
        float aspectRatio = 16.0f / 9.0f;
        float viewportHeight = 2.0f;
        float viewportWidth = aspectRatio * viewportHeight;
        float focalLength = 1.0f;

        Origin = new Vector3(0, 0, 0);
        Horizontal = new Vector3(viewportWidth, 0, 0);
        Vertical = new Vector3(0, viewportHeight, 0);
        HigherLeftCorner = Origin + Horizontal / 2 + Vertical / 2 + new Vector3(0, 0, focalLength);
    }
    public Ray GetRay(float u, float v)
    {
        return new Ray(Origin, HigherLeftCorner - u * Horizontal - v * Vertical + Origin);
    }
}

