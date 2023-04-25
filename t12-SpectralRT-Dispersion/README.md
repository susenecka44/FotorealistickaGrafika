# Topic12 - Spectral Ray-Tracer and Dispersion of Light
* You'll need to extend a **color definition** in the whole ray-tracer - it must
  be **multi-channel (spectral)**
  * eight to sixteen spectral components will suffice
  * you have to check your code
    (general `for`-loop over the whole `float[]` array must be used every single time
    while using or calculation color)
* All **color definitions** in your scene definition file should be
  * either **original `[R, G, B]` triplets** which must be converted to spectral representation
    using a color matching functions (see References)
  * or **spectral `float[]` tuples** with matching number of samples
* In the **pixel color finalization** ("pixel producer") you have to convert the spectral
  color to RGB using color matching functions (see References)
* For **light dispersion** you'll need a **variable refractive index** (as a function of
  wavelength - see References)

## Notes
* Spectral ray-tracers usually don't use stochastic sampling of the wavelength. Fixed
  spectral color representation is used instead.
* **A ray (starting from the primary ray)** represents all the wavelengths until it is refracted
  for the first time. At thit point it splits into individual (single-wavelength) rays,
  which willl never be further divided.
  * so basically two types of rays are used in a spectral ray-tracer: **multi-wavelength**
    ("white-light") rays and **single-wavelength** ("color") rays. The MW ray can be
    split into many SW rays, the SW ray remains the same

## References
* MFF slides
  * [Distributed Ray-Tracing](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-14-distributedrt.pdf)
* [Refractive index](https://en.wikipedia.org/wiki/Refractive_index) on Wikipedia
* [CIE 1931 color space](https://en.wikipedia.org/wiki/CIE_1931_color_space) on Wikipedia
* [CIE 1931 colour-matching functions](https://cie.co.at/datatable/cie-1931-colour-matching-functions-2-degree-observer-5nm)
  ([survey of all CIE data tables](https://cie.co.at/data-tables))

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
