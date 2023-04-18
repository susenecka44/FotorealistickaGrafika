# Topic08 - Acceleration
* Implement some **acceleration method** for ray-scene intersection
  * *scene division* (BVH - R-tree)
  * *3D space division* (KD-tree, Octree)
* Acceleration of **triangle mesh** object[s] is recommended 
  * large set of small spheres is suitable as well (e.g. particle system)

## Notes
* Build your acceleration data structure in **preprocessing stage**
  (see [parallelism](../06-Parallelism/README.md))

## References
* MFF slides
  * [Acceleration](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-11-acceleration.pdf)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
