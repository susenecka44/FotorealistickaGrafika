using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Ray
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


interface Camera
{
    public Ray GetRay(float u, float v);
}


public class PerspectiveCamera : Camera
{
    public Vector3 LowerLeftCorner { get; set; }
    public Vector3 Horizontal { get; set; }
    public Vector3 Vertical { get; set; }
    public Vector3 Direction { get; set; }
    public float FovDegrees { get; set; }
    public Vector3 Origin { get; set; }


    public PerspectiveCamera(Vector3 cameraPosition, int imageWidth, int imageHeight, float fovDegrees, Vector3 direction)
    {
        Origin = cameraPosition;
        Direction = Vector3.Normalize(direction);
        FovDegrees = fovDegrees;

        float theta = fovDegrees * (float)Math.PI / 180;
        float h = (float)Math.Tan(theta / 2);
        float viewportHeight = 2.0f * h;
        float aspectRatio = (float)imageWidth / imageHeight;
        float viewportWidth = aspectRatio * viewportHeight;

        Vector3 w = Vector3.Normalize(-Direction); // Assuming looking in the direction of the negative z-axis
        Vector3 up = new Vector3(0, -1, 0); 
        Vector3 u = Vector3.Normalize(Vector3.Cross(up, w));
        Vector3 v = Vector3.Cross(w, u);

        Horizontal = viewportWidth * u;
        Vertical = viewportHeight * v;
        LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - w;
    }

    public Ray GetRay(float u, float v)
    {
        return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
    }
}


