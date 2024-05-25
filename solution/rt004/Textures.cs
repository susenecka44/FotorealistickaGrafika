using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITexture
{
    Vector3d ColorAt(double u, double v, Vector3d p);
    Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal);
}

public class CheckerTexture : ITexture
{
    private Vector3d color1;
    private Vector3d color2;
    public double size;

    // biggger size = smaller squares
    public CheckerTexture(Vector3d color1, Vector3d color2, double size)
    {
        this.color1 = color1;
        this.color2 = color2;
        this.size = size;
    }

    public Vector3d ColorAt(double u, double v, Vector3d p)
    {
        double sines = Math.Sin(size * p.X) * Math.Sin(size * p.Y) * Math.Sin(size * p.Z);
        return sines < 0 ? color1 : color2;
    }
    public Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal)
    {
        // Return the input normal unmodified
        return normal;
    }
}
public class SolidTexture : ITexture
{
    private Vector3d color;

    public SolidTexture(Vector3d color)
    {
        this.color = color;
    }

    public Vector3d ColorAt(double u, double v, Vector3d p)
    {
        return color;
    }
    public Vector3d NormalAt(double u, double v, Vector3d p, Vector3d normal)
    {
        // Return the input normal unmodified
        return normal;
    }
}

