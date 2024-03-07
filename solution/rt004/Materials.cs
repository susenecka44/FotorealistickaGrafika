using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObjectMaterial
{
    //  “ kA + kD + kS = 1“ (to avoid overflow)  - neresim, ale asi TODO
    public float[] Color { get; private set; }
    public double kA { get; private set; } // ambient coef.
    public double kD { get; private set; } // difusion coef.
    public double kS { get; private set; } // specular coef.
    public double HighLight { get; private set; }
    public float Reflectivity { get; private set; }

    public float Refractivity { get; private set; }

    public ObjectMaterial(float[] color, double ka, double kd, double ks, double highLight, float reflectivity)
    {
        Color = color;
        kA = ka;
        kD = kd;
        kS = ks;
        HighLight = highLight;
        Reflectivity = reflectivity;
    }
}
