using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ObjectMaterial
{
    //  “ kA + kD + kS = 1“ (to avoid overflow)
    public float[] Color { get; private set; }
    public double kA { get; private set; }
    public double kD { get; private set; } // difusion coef.
    public double kS { get; private set; }
    public double HighLight { get; private set; }

    protected ObjectMaterial(float[] color, double ka, double kd, double ks, double highLight)
    {
        Color = color;
        kA = ka;
        kD = kd;
        kS = ks;
        HighLight = highLight;
    }
}

public class YellowMatt : ObjectMaterial
{
    public YellowMatt() : base(new float[] { 0.9f, 0.9f, 0.9f }, 0.1, 0.6, 0.4, 80){ }
}

public class BlueReflective : ObjectMaterial
{
    public BlueReflective() : base(new float[] {0.2f, 0.3f, 1.0f}, 0.1, 0.5, 0.5, 150) { }

}

public class RedReflective : ObjectMaterial
{

    public RedReflective() : base() { }
}