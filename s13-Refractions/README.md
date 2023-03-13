# Step13 - Refractions
* You have to add **transparent materials and refraction computation**
  to your Ray-tracer
  * you should extend your `shade()` function to include **refractions**.
  * you must update your **materials and BRDFs** to be able to define **transparency**.
  * you have to check your ray-solid intersection routines and define how they
    should handle **rays originating from iside of an object!**
    * this includes handling normal vectors correctly, e.g. for **internal reflections**.
    * it could be more about **"logic"** built on top of intersection functions...

## Notes
* Think about definition of **"material-to-material" interface**. Each material has
  its own index of refraction, there could be either single barrier between them or
  you can use two shapes, each with its own material.
   * the former solution allows scenes like "an air bubble inside of the glass/water"
   * the latter method is more simple to implement
* Don't forget to **reduce the light contribution of the refraction**
  (the reasonable multiplier should be something like `Ks` from Phong's BRDF - or
  Fresnel function approximation if you want to be more precise)
* Beware of **total internal reflection** - if the ray travels from more dense material,
  there is an output angle limit for refraction (**critical angle**). You must not
  use refraction above that angle.
* You can think about future **CSG scene** extension. Set operations define *shapes*
  (one *shape* can be defined by multiple *solids*), each *shape* has one material
  associated with it

## References
* **Refraction angle** computation -
  [slide #23 of the BRDF slideshow](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-05-brdf.pdf)
* **Fresnel term** for unpolarized light -
  [slide #38 of the BRDF slideshow](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-05-brdf.pdf)
  (angle "beta" is our angle - between reflected/incoming vector and normal)
* **Refractive index** - [Wikipedia page is sufficient](https://en.wikipedia.org/wiki/Refractive_index)
* **Critical angle and Total internal reflection** - [Wikipedia page is sufficient](https://en.wikipedia.org/wiki/Total_internal_reflection)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
