# Step14 - Anti-aliasing
* Your task is to modify your "image synthesizer"
  ("pixel producer") code to include **multiple
  samples/rays per pixel**.
* Any *unbiased method* will suffice, I'd recommend
  **Jittering** for its simplicity

## Notes
* Add **spp ("samples-per-pixel")** to your configuration file
  * if there is some specific condition about this number,
    use greater value if necessary
* If you haven't done this yet, I recommend creating
  an interface `PixelProducer` or `ImageSynthesizer`
  for abstraction of color computation of a pixel.
* Think about improving efficiency of your sampling algorithm
  - it is called *adaptive sampling* and will be addressed later
  (bonus points)

## References
* [Anti-aliasing and sampling slides](https://cgg.mff.cuni.cz/~pepca/lectures/pdf/prg-13-sampling.pdf)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
