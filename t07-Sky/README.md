# Topic07 - Sky
* Implement some interesting **sky-dome** function for your ray-tracer
  * clear sky (see some sky-dome models)
  * cloudy sky (simple turbulence-baased texture can be used)
  * night sky with stars (stochastic distribution) and moon
* Substitute **background color** (see the `shade()` function) by your sky function

## Notes
* Use **deterministic approaches** if possible (see [parallelism](../06-Parallelism/README.md))
  * mandatory for simulation of continuous phenomena (clouds)
* Keep in mind that stars have to be simulated like non-zero area objects. The star's
  magnitude must have an effect on the area and the intensity of the star's image
* Be inspired by references below, but don't restrict yourself, you can find more results
  online...

## References
* [Hosek & Wilkie model](https://cgg.mff.cuni.cz/projects/SkylightModelling/)
  (see the SIGGRAPH 2012 paper)
* [Tarantilis PhD](https://core.ac.uk/download/pdf/36695076.pdf) -
  this work looked at using the GPU to simulate clouds, but you can
  adapt some of the methods for offline rendering
* [P. Goswami's survey (2020)](https://link.springer.com/article/10.1007/s00371-020-01953-y)
* MFF slides
  * [Textures](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-12-textures.pdf)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
