# Step09 - Cleanup and OOP Refactor
* Your code is waiting for general cleanup and **Object-oriented refactor**.
* If your code already is OOP (public interfaces, classes with inheritance, etc.)
  then your work is done.
* Otherwise you should stop, look at your code with perspective, and try
  to reorganize it so that it'll be a joy to read and extend it.
* Think about future **persistence** of your objects (i.e. how will they be
  loaded from the configuration/scene-definition file).

## Notes
* Especially you should define a general interface for: `Solid`/`Shape` (perhaps something
  like `SceneNode` for future hierarchy), `BRDF` and associated `Materials`,
  `LightSource` and future `Texture` (all these entities could exist in your
  scene in multiple instances, so using **polymorphism is mandatory**).
* Of course `Camera` or `ImageSynthesizer` (`PixelProducer` if you like)
  will benefit from OO design as well.

## References
* Nothing yet

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
