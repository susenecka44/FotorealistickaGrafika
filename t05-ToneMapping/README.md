# Topic05 - Tone-mapping
* Add a **tone-mapping** feature to your ray-tracer
  * it will be able to save **LDR output image** (JPEF, PNG) together with HDR
* Choose some **tone-mapping** algorithm (even a simple one - logarithmic or
  sigmoidal)
  * consider using "range compression" on **luminance channel** only
  * color information (Hue, Saturation) could stay unchanged - it will result
    in more vibrant LDR colors

## Notes
* Use some 3rd party image library: `ImageSharp` is a good choice; you can use
  its `NuGet` version
* You can use either some form of **auto-exposure** or the level of exposure could
  be defined in the config file

## References
* MFF slides
  * [HDR graphics](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/icg-07-hdr.pdf)
    (in Czech)
* [ImageSharp](https://github.com/SixLabors/ImageSharp)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
