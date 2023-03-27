# Step12 - Scene hierarchy read from file
* You have to include **scene hierarchy** into your project
  * **Scene graph**
    * *inner nodes* with geometric transformations
    * *leaf nodes* with solids/shapes
    * *optional attributes* for inheritance of materials, colors,
      textures (future), etc.
* The whole scene should be stored in a **disk file**
  * you can use some form of **`include <file>`** directives if you like
    (will lead to a more versatile scene design because it
    avoids redundancies...)
  * your main program will read a scene definition in its startup,
    no hardwired data objects will be necessary

## Notes
* You have to redefine the **ray-scene intersection** function. You'll need
  direct and inverse transformation matrices at the graph edges
  * *direct transform* defines how the sub-solid is transformed before it is
    inserted to its parent node
  * *inverse transform* is used for ray transformation (ray is transformed
    into normalized solid/shape coordinate space, where all the formulas
    will be more simple)
* You can use **simple (flat)** storage or **native hierarchy structure**
  of your file-format
  * **flat storage** - all nodes are stored in one place (array, list), references have
    to be expressed explicitly using *"object id-s"* (similar to solid-material binding in my
    scene examples). Multiple links to a single node are allowed, but not easy to handle (*DAG*)
  * **native hierarchy structure** - you take advantage of the natural hierarchical structure
    of your data format (JSON, XML). Multiple links to a subtree are not possible (*tree*)
* **Geometric transforms** - you can use simple 4x4 matrices or allow to name
  transformations in some more readable way (see
  [SVG transform attribute](https://jenkov.com/tutorials/svg/svg-transformation.html))
* if you decide to use **attributes** in your scene graph, consider using
  simple **inheritance** (example: the `material=mat-id` attribute will
  apply not only to that node, but also to all child nodes until
  it is redefined...). You can evaluate all the attributes statically after the scene
  is read.

## References
* Nothing yet

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
