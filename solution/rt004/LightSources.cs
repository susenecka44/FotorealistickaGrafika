using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

public interface LightSource
{
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }

}
public class PointLight : LightSource
{
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }

    public PointLight(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}
