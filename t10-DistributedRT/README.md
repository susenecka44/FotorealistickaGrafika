# Topic10 - Distributed Ray-tracing
* Implement some **distributed** (stochastic) method[s] for your ray-tracer
  * **glossy reflections** (imperfect reflections defined by a "BRDF lobe")
  * **soft shadows** (area light sources)
  * **depth-of-field** simulation (realistic simulation of real lens leading to "bokeh")
  * more complicated cases will be defined later as separate tasks
    ([Motion blur](../t11-Animation-MotionBlur/README.md) or
    [Dispersion of light](../t12-SpectralRT-Dispersion/README.md))
* Every extension will be rewarded individually

## Notes
* **Universal principle** of distributed ray-tracing:
  some simple specific quantity (e.g. ray direction) is replaced by an integral of some
  function on suitable region. The function integral is used instead of the original value
  but the result is much harder to calculate.
  * an example: instead of a single shadow test ray, we need to calculate a set of "partial
    shadow rays" and calculate the partial shadowing factor from that set (not to mention
    the need to define the area of the light source and its sampling)
* Use **Monte-Carlo estimator** for integral calculation/approximation.
  * you will need a suitable **sampling method** (**jittering** is recommended in most cases)
  * 16 to 400 samples per pixel is sufficient in most cases, experiment with this value
    and try to find the reasonable ("turning") point
* You have two options how to implement **"the distribution"**
  * either you introduce a **one-to-many** fork into the calculation (potential for
    "exponential avalanche" if the recursion is affected)
  * or you can implement **"hidden sampling"** - every component involved could
    introduce its own stochastic calculation that does not increase
    the total number of calculated rays. In this case, all components of the ray-tracer
    must agree on a minimum number of samples per pixel.

## References
* MFF slides
  * [Distributed Ray-Tracing](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-14-distributedrt.pdf)
  * [Anti-aliasing and Sampling](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-13-sampling.pdf) -
    look for a suitable sampling method for your extension there

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
