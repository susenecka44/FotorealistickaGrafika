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

public class Camera
{
    public Vector3 Origin { get; set; }
    public Vector3 LowerLeftCorner { get; set; }
    public Vector3 Horizontal { get; set; }
    public Vector3 Vertical { get; set; }

    public Camera(Vector3 cameraPosition, int imageWidth, int imageHeight, float fovDegrees)
    {
        float aspectRatio = (float)imageWidth / imageHeight;

        // Given viewport height
        float viewportHeight = 10.0f; 

        // Calculate viewport width using the aspect ratio
        float viewportWidth = viewportHeight * aspectRatio;

        // Convert FOV from degrees to radians
        double fovRadians = (Math.PI / 180) * fovDegrees;

        // Assuming the FOV applies vertically, calculate the distance from the projection plane
        double focalLength = (viewportHeight / 2) / Math.Tan(fovRadians / 2);

        Origin = cameraPosition;
        Horizontal = new Vector3(viewportWidth, 0, 0);
        Vertical = new Vector3(0, viewportHeight, 0);
        LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - new Vector3(0, 0, (float)focalLength);
    }


    public Ray GetRay(float u, float v)
    {
        return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
    }
}


