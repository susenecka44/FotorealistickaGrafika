using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

public abstract class LightSource
{
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }

}

public class AmbientLight : LightSource
{
    public AmbientLight(Vector3 color, float intensity) {
        Position = new Vector3(0, 0, 0);
        float cX = color.X * intensity;
        float cY = color.Y * intensity;
        float cZ = color.Z * intensity;
        Color = new Vector3(cX, cY, cZ);
    }
}
public class PointLight : LightSource
{
    public PointLight(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}
