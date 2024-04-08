# Documentation of the r004 RayTracer

## Author

Julie Vondráčková

---

# Checkpoint IV. (tag `Chk IV`)

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

#### Command-Line Options

- `-w, --width`: Set the width of the output image (default: 600 pixels).
- `-h, --height`: Set the height of the output image (default: 400 pixels).
- `-f, --file`: Specify the output file name (default: `picture.hdr`).
- `-c, --config`: Path to the configuration file to use (default: `config.json`).

## Configuration File

The application can be customized using a JSON configuration file. Here's a detailed explanation of the parameters you can set in this file:

### General Settings

- `Width`: Width of the output image in pixels.
- `Height`: Height of the output image in pixels.
- `FileName`: Path and name of the output file, supporting `.hdr` and `.pfm` formats.

### Camera Settings

- `Position`: The camera's position in 3D space `[x, y, z]`.
- `Direction`: The direction the camera is pointing `[x, y, z]`.
- `BackgroundColor`: Background color `[R, G, B]` used when rays don't hit any object.
- `FOVAngle`: Field of view angle in degrees.

### Algorithm Settings

- `ShadowsEnabled`: Enable/disable shadow rendering.
- `ReflectionsEnabled`: Enable/disable reflections rendering.
- `RefractionsEnabled`: Enable/disable refractions rendering.
- `AntiAliasing`: Enable/disable anti-aliasing.
- `SamplesPerPixel`: Number of samples per pixel for anti-aliasing.
- `MaxDepth`: Maximum recursion depth for the ray tracing algorithm.
- `MinimalPerformance`: Threshold to avoid rendering artifacts.

### Materials

Defines materials with properties like color, reflectivity, and texture:

- `Name`: Unique identifier.
- `Color`: Base color `[R, G, B]`.
- `Ambient`, `Diffuse`, `Specular`: Lighting coefficients.
- `Shininess`: Specular highlight sharpness.
- `Reflectivity`: Reflection strength.
- `Refractivity`: Index of refraction.

### Lights

In the scene configuration, lights define how objects are illuminated, influencing shadows, reflections, and refractions. Each light has a specific type and set of properties.
- `Type`: Identifies the light type (`PointLight` or `AmbientLight`), determining its behavior in the scene.
- `Position`: (**PointLight** only) A vector `[x, y, z]` defining the light's position.
- `Color`: The light's color as `[R, G, B]`. Affects the scene's coloration from the light source.
- `Intensity`: (**AmbientLight** only) A scalar value that dictates the ambient light's strength, influencing the scene's overall brightness.

### Objects in Scene

Objects constitute the scene's visual elements. Each object is defined by its shape and properties, including size, position, material, and, for some objects, orientation.

#### Types of Objects

- **Sphere**: Defined by a center and radius, perfect for round elements.

- **Cube**: A polyhedron with square faces, useful for creating boxes and other blocky structures.

- **Plane**: An infinite two-dimensional surface, often used for floors, walls, or abstract surfaces.

#### Common Properties

- `Type`: The geometric shape of the object (`Sphere`, `Cube`, or `Plane`).

- `Position`: A vector `[x, y, z]` indicating the object's location in the scene.

- `Material`: References one of the materials defined in the `Materials` section, dictating the object's appearance.

#### Specific Properties

- `Radius`: (**Sphere** only) The sphere's size.

- `Size`: (**Cube** only) The cube's dimensions as `[width, height, depth]`.

- `Normal`: (**Plane** only) A vector `[x, y, z]` representing the plane's orientation through its normal vector.

- `Angle`: (**Cube** only) The cube's rotation angle around the Y-axis, allowing for orientation adjustments.

By customizing these properties, you can craft diverse scenes with varied lighting and object arrangements, offering vast creative possibilities in scene design.


## Algorithm

Raytracing algorithm in RayTracer.cs, Intersecting with objects and creating a HitRecord that keeps all data nessesary for calculating the colour of the hit object. (see code - I would say its nicely commented :])

## Bonuses

There are three structures - plane, sphere and cube.
Nicely done glass refractions - correct looking glass described:
'''
```
  "Name": "Glass",
  "Color": [ 0.8, 0.8, 1.0 ],
  "Ambient": 0.0,
  "Diffuse": 0.0,
  "Specular": 0.9,
  "Shininess": 150,
  "Reflectivity": 0.2,
  "Refractivity": 1.45
``` 
See Checkpoint4.pfm for generated sample scene (other images are from older versions - mostly without reflections, so their config files possibly dont work anymore)

- Sample Scene:
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/ddeb69b3-f53b-4550-b0f2-445d2d466a42)
configuration:
```
{
  "Width": 1000,
  "Height": 850,
  "FileName": "GeneratedImages/Checkpoint4.pfm",
  "CameraSettings": {
    "Position": [ 0.90, 1.00, -5.60 ],
    "Direction": [ 0.00, -0.17, 1.00 ],
    "BackgroundColor": [ 25, 50, 75 ],
    "FOVAngle": 40
  },
  "AlgorithmSettings": {
    "ShadowsEnabled": true,
    "ReflectionsEnabled": true,
    "RefractionsEnabled": true,
    "AntiAliasing": true,
    "SamplesPerPixel": 10,
    "MaxDepth": 7,
    "MinimalPerformance": 0.001
  },
  "Materials": [
    {
      "Name": "YellowMatt",
      "Color": [ 1.0, 1.0, 0.2 ],
      "Ambient": 0.1,
      "Diffuse": 0.8,
      "Specular": 0.2,
      "Shininess": 80,
      "Reflectivity": 0.05,
      "Refractivity": 0
    },
    {
      "Name": "BlueReflective",
      "Color": [ 0.8, 0.8, 1.0 ],
      "Ambient": 0.0,
      "Diffuse": 0.0,
      "Specular": 0.9,
      "Shininess": 150,
      "Reflectivity": 0.2,
      "Refractivity": 1.45
    },
    {
      "Name": "RedReflective",
      "Color": [ 0.8, 0.2, 0.2 ],
      "Ambient": 0.1,
      "Diffuse": 0.6,
      "Specular": 0.4,
      "Shininess": 80,
      "Reflectivity": 0.110,
      "Refractivity": 2.1
    },
    {
      "Name": "WhiteMatt",
      "Color": [ 0.9, 0.9, 0.9 ],
      "Ambient": 0.1,
      "Diffuse": 0.9,
      "Specular": 0.4,
      "Shininess": 80,
      "Reflectivity": 0.8,
      "Refractivity": 0
    },
    {
      "Name": "GreenMatt",
      "Color": [ 0.5, 0.9, 0.5 ],
      "Ambient": 0.2,
      "Diffuse": 0.6,
      "Specular": 0.2,
      "Shininess": 150,
      "Reflectivity": 0.20,
      "Refractivity": 0
    },
    {
      "Name": "Gold",
      "Color": [ 0.3, 0.2, 0.0 ],
      "Ambient": 0.2,
      "Diffuse": 0.2,
      "Specular": 0.8,
      "Shininess": 400,
      "Reflectivity": 0.90,
      "Refractivity": 0
    },
    {
      "Name": "Black",
      "Color": [ 0.01, 0.01, 0.01 ],
      "Ambient": 0.2,
      "Diffuse": 0.2,
      "Specular": 0.2,
      "Shininess": 20,
      "Reflectivity": 0.20,
      "Refractivity": 0
    }
  ],
  "ObjectsInScene": [
    {
      "Type": "Sphere",
      "Position": [ 0, 0, 0 ],
      "Radius": 1,
      "Material": "BlueReflective"
    },
    {
      "Type": "Sphere",
      "Position": [ 1, 0, 2 ],
      "Radius": 1,
      "Material": "YellowMatt"
    },
    {
      "Type": "Cube",
      "Position": [ 1.5, 1, 0 ],
      "Size": [ 0.6, 0.6, 0.6 ],
      "Material": "BlueReflective",
      "Angle": 30
    },
    {
      "Type": "Cube",
      "Position": [ 2, 0.2, 0 ],
      "Size": [ 0.4, 0.4, 0.4 ],
      "Material": "Gold",
      "Angle": 60
    },
    {
      "Type": "Plane",
      "Position": [ 0.0, -1.3, 0.0 ],
      "Normal": [ 0, 1, 0 ],
      "Material": "Black"
    }
  ],
  "Lights": [
    {
      "Type": "PointLight",
      "Position": [ -10.0, 8.0, -6.0 ],
      "Color": [ 255, 234, 231 ]
    },
    {
      "Type": "PointLight",
      "Position": [ 0.0, 20.0, -3.0 ],
      "Color": [ 255, 234, 231 ]
    },
    {
      "Type": "AmbientLight",
      "Color": [ 234, 234, 220 ],
      "Intensity": 0.5
    }
  ]
}

```

### Use of AI

AI used for mostly fixing parts of code - debugging and small help.

Checkpoint IV.
- https://chat.openai.com/share/3253c7c8-56ea-4006-9030-ae4d3c33d87b

Checkpoint III.
- https://chat.openai.com/share/f69f1015-9448-4923-be62-176c46bcb8c8
- https://chat.openai.com/share/af8f2f21-3dc3-4ae0-be45-cb0791f8a29c
- https://chat.openai.com/share/7a216118-ae1c-43f5-8079-bb3a7cce1a35
- https://chat.openai.com/share/8c335fb4-eeb5-4717-b9da-51d143d67c62
