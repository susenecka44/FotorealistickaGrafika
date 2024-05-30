using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

/// <summary>
/// Object material class that represents the material of an object
/// -> all the properties of the material are stored in this class
/// </summary>
public class ObjectMaterial
{
    //  “ kA + kD + kS = 1“ (to avoid overflow)
    public double[] Color { get; private set; }
    public double[] SecondaryColor { get; private set; }
    public ITexture Texture { get; private set; }
    public double kA { get; private set; } // ambient coef.
    public double kD { get; private set; } // difusion coef.
    public double kS { get; private set; } // specular coef.
    public double HighLight { get; private set; }
    public double Reflectivity { get; private set; }
    public double Refractivity { get; private set; }

    public ObjectMaterial(double[] color, double[] secondary_color, ITexture texture, double ka, double kd, double ks, double highLight, double reflectivity, double refractivity)
    {
        Color = color;
        SecondaryColor = secondary_color;
        Texture = texture;
        kA = ka;
        kD = kd;
        kS = ks;
        HighLight = highLight;
        Reflectivity = reflectivity;
        Refractivity = refractivity;
    }

    /// <summary>
    /// Returns the color of the object material at the given point
    /// </summary>
    /// <param name="u"> coordinate of the point </param>
    /// <param name="v"> coordinate of the point </param>
    /// <param name="p"> used for texture </param>
    /// <returns></returns>
    public Vector3d GetColor(double u, double v, Vector3d p)
    {
        if (Texture != null)
            return Texture.ColorAt(u, v, p);
        return new Vector3d(Color[0], Color[1], Color[2]);
    }
}
