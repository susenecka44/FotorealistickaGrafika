# Step04 - Camera (ray generator) for ray-tracing
* Your have to implement simple perspective camera which
  is able to generate ray for arbitrary pixel on the projection plane.
  More useful `Camera` implementation will use `double` coordinates
  on the projection plane (instead of `int` pixel coordinates).
* Usual (highly recommended!) ray representation is: `Vector3d P0, Vector3d p1`
  where `P0` is ray's origin and `p1` is its direction vector. Note that
  although these two "vectors" have the same C# type, they
  represent different mathematical objects (`P0` is a point in 3D space,
  `p1` is a 3D vector/direction).
* Define some simple but useful way how to define Camera parameters, we'll
  want to be able to look from an arbitrary point to an arbitrary direction.
  Viewing angle (frustum) should be defined as well, together with the raster-image
  dimension in pixels (you may assume square pixels)

## Notes
* Today you can hard-wire your first camera implementation to your code but
  clever programmers use to think about format for camera persistence from
  the beginning (I mean config-file representation of your camera)
* You can think about more complicated cameras (ray-generators): fish-eye,
  cylindrical perspective etc. You don't need to implement them right away
  but to think about them could be useful...
* You don't need to use object-oriented design today at all, but in the near
  future you will be asked for it (see the [step08](../step08)).
