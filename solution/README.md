# Documentation of the r004 RayTracer

## Author

Julie Vondráčková

---

# Checkpoint III. (tag `Chk III`)

Creating a scene with lights, refractions, shadows.

### Command Line Arguments

List all the arguments here, with default values.

| Argument | Description | Default Value |
|----------|-------------|---------------|
| `-w`, `--width` | Image width. | `600` |
| `-h`, `--height` | Image height. | `400` |
| `-f`, `--file` | Output file name. | `"picture.hdr"` |
| `-c`, `--config` | Configuration file path. | |

## Input Data

### Camera Settings

This section controls the virtual camera's settings within your scene.

- **Position**: Specifies the camera's location as `[x, y, z]` coordinates.
- **Direction**: Determines where the camera is pointing, using a vector `[x, y, z]`.
- **BackgroundColor**: Sets the background color in RGB `[R, G, B]`, with values ranging from 0 to 255.
- **FOVAngle**: The field of view angle in degrees, which determines the observable world extent.

### Algorithm Settings

Adjustments here affect the rendering algorithm's performance and output quality.

- **ShadowsEnabled**: If `true`, enables shadow casting for objects in the scene.
- **ReflectionsEnabled**: If `true`, enables reflective effects on surfaces.
- **SamplesPerPixel**: The number of samples per pixel, influencing anti-aliasing quality.
- **MaxDepth**: The maximum recursion depth for rays, affecting reflection and refraction accuracy.
- **MinimalPerformance**: A performance optimization threshold. Effects below this value may be simplified.

### Materials

Defines the appearance of objects with properties like color and reflectivity.

- **Name**: A unique identifier for the material.
- **Color**: The material's base color, in `[R, G, B]` format, with values from 0 to 1.
- **Ambient**: The ambient light reflection coefficient.
- **Diffuse**: The diffuse reflection coefficient, affecting light scatter.
- **Specular**: The specular reflection coefficient, influencing shininess.
- **Shininess**: Determines the sharpness of specular highlights.
- **Reflectivity**: Ranges from 0 (non-reflective) to 1 (perfect mirror), indicating the material's reflectiveness.

### Objects in Scene

Describes geometric objects placed in the scene.

- **Type**: The object type (e.g., "Sphere", "Plane").
- **Position**: The object's `[x, y, z]` coordinates.
- **Radius**: For spheres, the sphere's radius.
- **Normal**: For planes, the normal vector `[x, y, z]` indicating orientation.
- **Material**: The name of the material applied, corresponding to one of the defined materials.

### Lights

Configures scene lighting, including point and ambient lights.

- **Type**: Light source type (e.g., "PointLight", "AmbientLight").
- **Position**: For point lights, the light's `[x, y, z]` position.
- **Color**: The light color in `[R, G, B]`, with values from 0 to 255.
- **Intensity**: For ambient lights, sets the scene's overall light brightness.

## Algorithm

Raytracing algorithm in RayTracer.cs, Intersecting with objects and creating a HitRecord that keeps all data nessesary for calculating the colour of the hit object. (see code - I would say its nicely commented :])

## Bonuses

There are three structures - plane, sphere and cube.

See Image.hdr for generated sample scene (other images are from older versions - mostly without reflections, so their config files possibly dont work anymore)

### Use of AI

AI used for mostly fixing parts of code - debugging and small help.

- https://chat.openai.com/share/f69f1015-9448-4923-be62-176c46bcb8c8
- https://chat.openai.com/share/af8f2f21-3dc3-4ae0-be45-cb0791f8a29c
- https://chat.openai.com/share/7a216118-ae1c-43f5-8079-bb3a7cce1a35
- https://chat.openai.com/share/8c335fb4-eeb5-4717-b9da-51d143d67c62
