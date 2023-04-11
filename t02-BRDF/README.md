# Topic02 - More reflectance functions (BRDF)
* You can implement more **local reflectance functions**
  * see [technical details](../s06-BRDF/README.md) from previous step
  * you must implement associated **materials** (material data objects)
    defining attributes needed for your specific reflectance compting
  * you have to implement **BRDF & material persistence** (it will be read from a scene file)

## Notes
* **Microfacet-based BRDFs**: Cook-Torrance, Oren-Nayar and many further improvements
* Layered reflectance models, e.g. **Weidlich-Wilkie**

## References
* MFF slides
  * [Reflectance Models (BRDF)](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-05-brdf.pdf)
* Eric Lafortune et al.: [Non-Linear Approximation of Reflectance Functions](https://dl.acm.org/doi/10.1145/258734.258801),
  LFTG 1997
* Ivo Kabel's [Rendering Layered Materials](https://ivokabel.github.io/2018/05/15/rendering-layered-materials.html)
* [Weidlich-Wilkie Layered Model](https://ivokabel.github.io/2018/05/16/rendering-layered-materials-weidlich-wilkie-layered-model.html)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
