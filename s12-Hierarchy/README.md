# Step12 - Scene hierarchy read from file
* You have to include **scene hierarchy** into your project
  * **Scene graph**
    * *inner nodes* with geometric transformations
    * *leaf nodes* with solids
    * *optional attributes* for inheritance of materials, colors,
      textures (future), etc.
* The whole scene should be stored in a **disk file**
  * you can use some form of **`include <file>`** directives if you like
    (will lead to a more versatile scene design because it
    avoids redundancies...)
  * your main program will read a scene definition in its startup,
    no hardwired data objects will be necessary

## Notes
* Nothing yet

## References
* Nothing yet

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
