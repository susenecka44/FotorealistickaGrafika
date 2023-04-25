# Topic11 - Scene Animation and Motion Blur
* For implementation of **motion blur** we need to define scene motion/animation first...
  * basically we need to imagine that the scene **changes in time**. One of the simplest ways
    is to redefine **node's geometric transformation** in the scene hierarchy (see
    [Scene hierarchy](../s12-Hierarchy/README.md))
  * transformations (or any other quantities in the scene definition) could
    be **time-dependent**
  * you have to extend the **scene-definition format** to reflect that change and
  * re-implement **scene rendering algorithm** (especially if you are using
    [parallelism](../t06-Parallelism/README.md))
* The final change will affect the **image-synthesizer** (pixel-producer) - individual
  samples inside the pixel should have stochastic times, defined from a suitable **interval of
  exposure**
* Computing of an **Animation sequence** - a sequence is just a list of individual frames, computed independently.
  You have to extend your `Main` function to output a sequence of LDR images (see
  [Tone-mapping extension](../t05-ToneMapping/README.md))
  * you can use [FFmpeg program](https://ffmpeg.org/) to produce the final video file from
    a sequence of images

## Notes
* **Parallelism** - each "Worker" must have its own copy of the scene data (because
  each worker may need to set their own "simulated time")
* **Animation sequence** implementation is not mandatory, and will be rewarded by
  extra bonus points

## References
* MFF slides
  * [Distributed Ray-Tracing](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-14-distributedrt.pdf)
  * [Anti-aliasing and Sampling](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-13-sampling.pdf) -
    we will need a 1D sampling only (jittering is recommended)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
