# Topic03 - Textures
* You have to design and implement a **texture system** for your ray-tracer
  * allow texture to modify/modulate at least **surface color** and **normal vector**
    (normal map)
  * more options: modulation of **material attributes**
* Revisit [intersection details](../s05-Solids/README.md) from previous step
* You have to implement **texture persistence** (it will be read from a scene file)
* You should use **2D texture coordinates** when it makes sense
  (*3D texture concept* can be more straightforward in other cases)

## Notes
* I recommend storing all *intersection data* in a special data object (`Intersection`).
  Texture objects will be able to modify the `Intersection` prior to its actual use
  in the `shade()` function...
* Use **formulae** for simple geometric textures (checkerboard, polka dots,
  stripes, concentric circles, etc.)

## References
* MFF slides
  * [Textures](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-12-textures.pdf)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
