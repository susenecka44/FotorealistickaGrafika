# Topic04 - Textures based on noise functions
* You have to implement some **noise functions** first
  * Ken Perlin's [Noise](https://en.wikipedia.org/wiki/Perlin_noise) is a good start
  * see more methods in references
* You have to implement **texture persistence** (it will be read from a scene file)
* You should use **2D texture coordinates** when it makes sense
  (*3D texture concept* can be more straightforward in other cases)
* All [previously noted details](../t03-Textures/README.md) apply here as well

## Notes
* Use **independent noise functions** in methods where two or more noise functions
  are used
  * a set of many **different hashing functions** is recommended (at least use some
    simple schema where you can substitute different prime numbers etc...)
  * different **random seed** (defined deterministically) is another good example
* It is recommended to use **3D texture concept** for textures like "wood" or
  "marble"

## References
* MFF slides
  * [Textures](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-12-textures.pdf)
* [Perlin noise](https://en.wikipedia.org/wiki/Perlin_noise)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
