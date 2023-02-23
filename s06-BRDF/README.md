# Step06 - Reflectance function (a.k.a. BRDF) and materials
* Your have to implement simple **reflectance function** together with
  **material definition** for solids in your scene. **Phong** method
  is my recommendation.
* The reflectance function has to compute a **contribution of a light
  source** to the color visible on the surface of a solid. Usual
  parameters are: **normal vector** `Vector3D n` of the surface point,
  **light vector** `Vector3D l` pointing towards the light source
  and **view vector** `Vector3D v` pointing towards the viewer (camera,
  previous ray-scane intersection...). Material associated with the solid
  will be available as well.
* Regarding the **result value**, the recommended semantics are
  a *relative color coefficient*. You must multiply it by
  the intensity/radiance of the incoming light to get the actual
  color.
* Think about the **persistence** of your materials (i.e. how will they be
  loaded from the configuration/scene-definition file in the future).

## Notes
* Consider **sharing material definition** among solids in the scene.
* Consider **separating the ambient component** from the "real" ones.
  The regular components of reflection (diffuse and specular)
  add up for multiple light sources, but this must not be done
  with the "ambient term". You can solve this problem elegantly
  using  a special **ambient light source**.
* You don't have to use object-oriented design today.
  Remember that in the near future you will be required to (see
  the [step08](../step08)).

## References
* MFF slides
  * [The Phong Shading Model](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-03-phong.pdf)
  * [Reflectance Models (BRDF)](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-05-brdf.pdf) -
    see slide 55 (Schlick's substitute for the Fresnel term)
* [BRDF on Wikipedia](https://en.wikipedia.org/wiki/Bidirectional_reflectance_distribution_function)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
