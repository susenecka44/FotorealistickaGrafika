# Step10 - Shadows
* You have to include **simple ("sharp") shadows** computed
  by single shadow rays.
* This is done by throwing **secondary "shading" rays** from each
  intersection point to each light source. The contribution of the
  light source is counted only if no obstacle is found!

## Notes
* Beware of numerical/rounding errors: **the solid must not
  intersect its own shading ray** (use of some suitable constant
  is recommended, e.g. `double EPSILON = 1.0e-6`) for
  parameter `t` tests.
* If you are using **directional light sources**
  ("shining from infinity"), their obstacle presence test could
  be different.
* Consider using **unit direction vectors** for your shading rays.
  It could be little more complicated (distance of the light source),
  but can lead to more robust computation
* Think about future extension "soft shadows"

## References
* Nothing yet

# Sample
A sample scene with three spheres and two point light sources
is provided for reference: [scene definition](../s08-ch2-RTimage/sample-scene.md).
You should get similar result if you use the same scene definition.

![Sample result](sample-raycasting-shadows.jpg)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
