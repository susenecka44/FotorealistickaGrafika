# Step07 - Light Source
* Your have to implement at least one simple **light source**. It will cooperate
  with your BRDF and contribute light to the image.
* Your options are **Point light source** or **Directional light source**
  The former is defined by its **Position** in 3D space and its **Intensity/Color**.
  The latter has a **direction vector** instead of a position.
* Think about the **persistence** of your light sources (i.e. how will they be
  loaded from the configuration/scene-definition file in the future).

## Notes
* Consider **separating the ambient component** of your BRDF
  from the "real" ones.
  The regular components of reflection (diffuse and specular)
  add up for multiple light sources, but this must not be done
  with the "ambient term". You can solve this problem elegantly
  using  a special **ambient light source**. Such a light source won't
  have a position or direction vector.
* You don't have to use object-oriented design today.
  Remember that in the near future you will be required to (see
  the [step09](../s09-OOP)).

## References
* MFF slides
  * [The Phong Shading Model](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-03-phong.pdf) -
    see slide #12 (Compensation of Light Distance)
* [BRDF on Wikipedia](https://en.wikipedia.org/wiki/Bidirectional_reflectance_distribution_function)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
