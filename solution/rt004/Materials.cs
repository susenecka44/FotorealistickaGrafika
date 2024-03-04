using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Material
{
    public float[] Color { get; private set; }
    public double kA { get; private set; }
    public double kD { get; private set; }
    public double kS { get; private set; }
    public double HighLight { get; private set; }

    protected Material(float[] color, double ka, double kd, double ks, double highLight)
    {
        Color = color;
        kA = ka;
        kD = kd;
        kS = ks;
        HighLight = highLight;
    }
}

public class WhiteMatt : Material
{
    public WhiteMatt() : base(new float[] { 0.9f, 0.9f, 0.9f }, 0.1, 0.6, 0.4, 80)
    {
        // else
    }
}
