# Step05 - Solids
* Your have to implement **two solids** for your future ray-tracer. **Sphere**,
  **cylinder**, **plane** (or a huge rectangle) are recommended options.
* The crucial function for your solids will be **the intersection
  function**. Given a ray in parametric form (`Vector3D P0, Vector3d p1`),
  this function calculates the intersection. You probably want to work
  with the "parametric" intersections - `P0 + t * p1`, the actual
  real-world coordinates can be calculated later.
* Think about the **persistence** of your solids (i.e. how will they be
  loaded from the configuration/scene-definition file in the future).

## Notes
* For some future extensions, it will be necessary to include
  **2D texture coordinates**. Think about it today -
  you don't have to implement it right away, but be prepared for it...
* You don't have to use object-oriented design today.
  Remember that in the near future you will be required to (see
  the [step08](../step08)).

## References
* MFF slides
  * [Ray-Scene Intersections](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-08-intersection.pdf)
* Dave Eberly's [Geometric Tools](https://www.geometrictools.com/), see the
  [Documentation section](https://www.geometrictools.com/Documentation/Documentation.html)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
