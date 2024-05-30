# Pilot source code
You can use the files in this directory as a starting point for your
own solution.

## The `src` directory
Includes the `rt004` project - a command-line program that can create
HDR files. This is a good starting point for your future raytracer.

Used platform is `.NET 6.0 Command line` with two simple helpers:
* `FloatImage.cs` - HDR raster image stored in memory, able to export to
  the `.pfm` format.
* Mathematics package `OpenTK.Mathematics` for simple types
  (vectors, matrices...) based on the `double` floating point type.

## The `shared` directory
The `FloatImage.cs` file is the only support file so far.
