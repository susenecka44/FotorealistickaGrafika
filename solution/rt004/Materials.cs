using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


public class ObjectMaterial
{
    //  “ kA + kD + kS = 1“ (to avoid overflow)
    public double[] Color { get; private set; }
    public double kA { get; private set; } // ambient coef.
    public double kD { get; private set; } // difusion coef.
    public double kS { get; private set; } // specular coef.
    public double HighLight { get; private set; }
    public double Reflectivity { get; private set; }
    public double Refractivity { get; private set; }

    public ObjectMaterial(double[] color, double ka, double kd, double ks, double highLight, double reflectivity, double refractivity)
    {
        Color = color;
        kA = ka;
        kD = kd;
        kS = ks;
        HighLight = highLight;
        Reflectivity = reflectivity;
        Refractivity = refractivity;
    }
}
