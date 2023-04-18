# Topic08 - Acceleration
* Implement some **acceleration method** for ray-scene intersection
  * *scene division* (BVH - R-tree)
  * *3D space division* (KD-tree, Octree)
* Acceleration of **triangle mesh** object[s] is recommended 
  * large set of small spheres is suitable as well (e.g. particle system)

## Notes
* Build your acceleration data structure in **preprocessing stage**
  (see [parallelism](../06-Parallelism/README.md))
* Most advanced solutions should use some form of **tree optimisation**
  (e.g."SAH heuristics")

## References
* [R. Wiche's article about SAH](https://medium.com/@bromanz/how-to-create-awesome-accelerators-the-surface-area-heuristic-e14b5dec6160)
* MFF slides
  * [Acceleration](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-11-acceleration.pdf) - includes
    SAH as well
* [Line vs. Box (Eberly)](https://www.geometrictools.com/Documentation/IntersectionLineBox.pdf)
* [Ray vs. Triangle (Moller-Trumbore)](https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
