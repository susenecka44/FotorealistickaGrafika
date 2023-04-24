# Topic09 - Constructive Solid Geometry (CSG)
* You have to implement [scene hierarchy](../s12-Hierarchy/README.md) first
* Then you add **set operations** as scene-nodes' capability
* Logic of the **ray-scene intersection** should be redefined to conform with all set operation rules
  * **Union** (nothing new) - the nearest intersection is the nearest of
    all available intersections (i.e. in all sub-trees)
  * **Subtraction** - the nearest intersection of a positive component if it is outside of a subtracted one,
    end of a negative component if it is inside a positive one...
  * **Intersection** - the nearest positive intersection which lies inside all other components

## Notes
* You have to compute **all intersections** for a specific solid (not only the nearest one)!
* You can apply simple set **rules** to **sorted lists of intersections** (see X-transition pseudocode
  referenced below)
* Don't forget to reverse normal vectors for **"negative operations"** (Subtraction, Intersection)
  * additional attribute `bool Front` is recommended (`true` means we are entering the `Solid`/`Shape`-s
    front side at this point). If `Front` is `false`, we have to reverse the normal
    vector's orientation
* Naturally **thin** solids/shapes (e.g. individual spline surfaces...) need to be redefined
  * either as **"infinite thin shells"** (every ray-surface intersection generates two adjacent
    "intersection events" - the former one **enters** the "solid", the latter one **leaves** it...)
  * or you can tweak **the concept of the interior of a solid** a bit (every intersection with the
    **front side** of the surface is considered **entering**, every intersection with the
    **back side** is considered **leaving** the "solid"). Such "solid" seems ill-defined
    but in practice it might work

## References
* MFF slides
  * [Intersections](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-08-intersection.pdf) - slides
    20 to 23
  * [Image coding](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/icg-19-imagecoding.pdf) - slides
    13 to 15 are about *X-transition list* with pseudocode for set operations (direct equivalent
    to our ray vs. CSG situation)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
