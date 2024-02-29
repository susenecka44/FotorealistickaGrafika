using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

interface LightSource
{

}

public class PointLight : LightSource
{
    public Vector3 Position { get; }
    public Vector3 Color { get; }

    public PointLight(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}
