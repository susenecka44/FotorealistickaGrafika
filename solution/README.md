# Documentation of the r004 RayTracer - renderer

## Author

Julie Vondráčková

# Description
A program that creates a scene with lights, refractions, shadows.

---
### New updates
Added texture mapping :]
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/d5f3201f-043a-45ad-acdd-47fb72ce130e)


### Command Line Arguments

List all the arguments here, with default values.

| Argument | Description | Default Value |
|----------|-------------|---------------|
| `-w`, `--width` | Image width in pixels. | `600` |
| `-h`, `--height` | Image height in pixels. | `400` |
| `-f`, `--file` | Output file name. | `"picture.pfm"` |
| `-c`, `--config` | Configuration file path. | `"config.json"` |

## Configuration File

The application can be customized using a JSON configuration file. 

Here's a detailed explanation of the parameters you can set in this file:

*(if some feature is not set, it gets automatically set to its default value)*

### General Settings

- `Width`: Width of the output image in `pixels`. *Default: 600*
- `Height`: Height of the output image in `pixels`. *Default: 400*
- `FileName`: Path and name of the output file, supporting `.hdr` and `.pfm` formats. *Default: picture.pfm*

### Camera Settings

Configuration for the scene's camera.
- `Position`: The camera's position in 3D space `[x, y, z]`. *Default: [0.0, 0.0, 5.0]*
- `Direction`: The direction the camera is pointing `[x, y, z]`. *Default: [0.0, 0.0, -1.0]*
- `BackgroundColor`: Background color `[R, G, B]` used when rays don't hit any object. *Default: [0.2, 0.2, 0.2]*
- `FOVAngle`: Field of view `angle in degrees`. *Default: 90.0*

### Algorithm Settings

Defines settings for the rendering algorithm.

- `ShadowsEnabled`: Enable/disable shadow rendering (`true/false`). *Default: true*
- `ReflectionsEnabled`: Enable/disable reflections rendering (`true/false`). *Default: true*
- `RefractionsEnabled`: Enable/disable refractions rendering (`true/false`). *Default: true*
- `AntiAliasing`: Chooses the type of Antialiasing algorithm *Default: JitteredSamplingAliasing*
  - `NoAliasing`
  - `HammersleyAliasing`
  - `CorrelatedMultiJitteredAliasing`
  - `SupersamplingAliasing`
  - `JitteredSamplingAliasing`
- `SamplesPerPixel`: `Number` of samples per pixel for anti-aliasing. *Default: 5*
- `MaxDepth`: Maximum `recursion depth` for the ray tracing algorithm. *Default: 5*
- `MinimalPerformance`: `Threshold` to avoid rendering artifacts. *Default: 0.0*
- `RayTracer`: `Type` of ray tracer algorithm used.*Default: Basic*
   - `Basic`

### Materials

Defines the visual characteristics of object surfaces.

- `Name`: Unique identifier. *Default: Default*
- `Color`: Base color `[R, G, B]`. *Default: [1.0, 1.0, 1.0]*
- `Ambient`: Ambient reflectance coefficient. *Default: 0.1*.
- `Diffuse`: Diffuse reflectance coefficient. *Default: 0.9*.
- `Specular`: Specular reflectance coefficient. *Default: 0.5*.
- `Shininess`: Specular highlight sharpness. *Default: 200.0*
- `Reflectivity`: Reflection strength. *Default: 0.0*
- `Refractivity`: Index of refraction. *Default: 0.0*

### Lights

In the scene configuration, lights define how objects are illuminated, influencing shadows, reflections, and refractions. Each light has a specific type and set of properties.

- `Type`: Identifies the light type, determining its behavior in the scene. *Default: PointLight*
   - `PointLight`
   - `AmbientLight`
- `Position`: (**PointLight** only) A vector `[x, y, z]` defining the light's position. *Default: [0.0, 10.0, 0.0]*
- `Color`: The light's color as `[R, G, B]`. Affects the scene's coloration from the light source. *Default: [1.0, 1.0, 1.0]*
- `Intensity`: A scalar value that dictates the ambient light's strength. *Default: 1.0*

### Objects and Scene

Objects constitute the scene's visual elements. Each object is defined by its shape and properties, including size, position, material, and, for some objects, orientation.

#### Types of Primitive Objects

Represents basic geometric shapes for constructing objects.

- **Sphere**: Defined by a center and radius, perfect for round elements.
- **Cube**: A polyhedron with square faces, useful for creating boxes and other blocky structures.
- **Plane**: An infinite two-dimensional surface, often used for floors, walls, or abstract surfaces.
- **Cylinder**: An circular tube-like looking shape, often used for columns, glasses, vases etc.

##### Common Properties

- `Type`: The geometric shape of the object (`Sphere`, `Cube`, `Cylinder`, or `Plane`). *Default: Sphere*
- `Position`: A vector `[x, y, z]` indicating the object's location in the scene. *Default: [0.0, 0.0, 0.0]*
- `Material`: References one of the materials by `Name defined in the Materials` section, dictating the object's appearance. *Default: Default*

##### Specific Properties

- `Radius`: (**Sphere** adn **Cylinder** only) `Float` specifying the sphere's size, and the radius of the cylinder. * Default: 1.0*
- `Size`: (**Cube** only) The cube's dimensions as `[width, height, depth]`. *Default: [1.0, 1.0, 1.0]*
- `Normal`: (**Plane** only) A vector `[x, y, z]` representing the plane's orientation through its normal vector. *Default: [0.0, 1.0, 0.0]*
- `Angle`: (**Cube** only) The cube's rotation angle around the Y-axis, allowing for orientation adjustments.*Default: 0.0*
- `Height`: (**cylinder** only) Height, of the cylinder. *Default: 1.0*.
- `Scale`: Scaling factors for **nested objects**. *Default: [1, 1, 1]*.
- `Rotation`: Rotation angles for **nested objects**. *Default: [0, 0, 0]*.

#### Objects In Scene

Represents complex objects composed of one or more `PrimitiveObject`.

- `Name`: Name of the complex object. *Default: Object*.
- `BasicShapes`: List of `PrimitiveObject` constituting the complex object. Initialized as empty. *Default: empty*

#### Scene

Specifies instances of objects placed within the scene.

- `Type`: Type of object being placed. *Default: Object*
- `Position`: 3D position of the object within the scene. *Default: [0, 0, 0]*
   - Adds the X, Y and Z values to the parent object defined in `ObjectsInScene`
- `Scale`: Scaling factors applied to the object. *Default: [1, 1, 1]*
   - Three dimensional scaling avaiable only for `cubes`, the rest only by the first value
- `Rotation`: Rotation angles applied to the object. *Default: [0, 0, 0]*
  - Rotating only around the centers of the `primitive objects`


By customizing these properties, you can craft diverse scenes with varied lighting and object arrangements, offering vast creative possibilities in scene design.

## Algorithm

Raytracing algorithm in RayTracer.cs, Intersecting with objects and creating a HitRecord that keeps all data nessesary for calculating the colour of the hit object. (see code - I would say its nicely commented :])

## Bonuses

- There are four basic structures - plane, sphere, cylinder and cube.
- There is an option for object hierarchy - no more copying the basic shapes :]
- Four types of antialiasing techniques: Hammersley Aliasing, Correlated Multi-Jittered Aliasing, Supersampling Aliasing and Jittered Sampling Aliasing
- Nicely done glass refractions - correct looking glass described:
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


- Sample Scene:
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/be882fb2-1d20-4526-9ac3-8651caa92336)

configuration:

```
{
  "Width": 800,
  "Height": 650,
  "FileName": "GeneratedImages/forrest.pfm",
  "CameraSettings": {
    "Position": [ 0.90, 1, -7 ],
    "Direction": [ 0.00, -0.17, 1.00 ],
    "BackgroundColor": [ 25, 50, 75 ],
    "FOVAngle": 40
  },
  "AlgorithmSettings": {
    "ShadowsEnabled": true,
    "ReflectionsEnabled": true,
    "RefractionsEnabled": true,
    "SamplesPerPixel": 10,
    "MaxDepth": 7,
    "MinimalPerformance": 0.001,
    "AntiAliasing": "HammersleyAliasing",
    "RayTracer": "basic"
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
      "Name": "Leaves",
      "Color": [ 0.003921, 0.196, 0.12549 ],
      "Ambient": 0.2,
      "Diffuse": 0.9,
      "Specular": 0.00000001,
      "Shininess": 10,
      "Reflectivity": 0,
      "Refractivity": 0
    },
    {
      "Name": "Trunk",
      "Color": [ 0.64, 0.2, 0.16 ],
      "Ambient": 0.1,
      "Diffuse": 0.8,
      "Specular": 0.2,
      "Shininess": 80,
      "Reflectivity": 0.05,
      "Refractivity": 0
    },
    {
      "Name": "Grass",
      "Color": [ 0.329, 0.722, 0.278 ],
      "Ambient": 0.2,
      "Diffuse": 0.2,
      "Specular": 0.02,
      "Shininess": 1,
      "Reflectivity": 0.01,
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
      "Name": "Tree",
      "BasicShapes": [
        {
          "Type": "Cylinder",
          "Position": [ 0, -1, 0 ],
          "Height": 1.2,
          "Radius": 0.1,
          "Material": "Trunk"
        },
        {
          "Type": "Sphere",
          "Position": [ 0, 0.7, 0 ],
          "Radius": 0.8,
          "Material": "Leaves"
        },
        {
          "Type": "Sphere",
          "Position": [ 0.5, 0.6, 0 ],
          "Radius": 0.4,
          "Material": "Leaves"
        },
        {
          "Type": "Sphere",
          "Position": [ -0.2, 0.9, 0 ],
          "Radius": 0.6,
          "Material": "Leaves"
        }
      ]
    },
    {
      "Name": "YellowSphere",
      "BasicShapes": [
        {
          "Type": "Sphere",
          "Position": [ 0, 0, 0 ],
          "Radius": 1,
          "Material": "YellowMatt"
        }
      ]
    },
    {
      "Name": "Floor",
      "BasicShapes": [
        {
          "Type": "Plane",
          "Position": [ 0, 0, 0 ],
          "Normal": [ 0, 1, 0 ],
          "Material": "Grass"
        }
      ]
    },
    {
      "Name": "GlassCube",
      "BasicShapes": [
        {
          "Type": "Cube",
          "Position": [ 0, 0, 0 ],
          "Size": [ 1, 1, 1 ],
          "Material": "BlueReflective",
          "Angle": 30
        }
      ]
    }
  ],
  "Scene": [
    {
      "Type": "Tree",
      "Position": [ -2, 0, 0 ],
      "Scale": [ 1, 1, 1 ]
    },
    {
      "Type": "Tree",
      "Position": [ 2, 0, 0 ],
      "Scale": [ 1, 1, 1 ]
    },
    {
      "Type": "Tree",
      "Position": [ -2, 0, -2 ],
      "Scale": [ 0.8, 0.8, 0.8 ]
    },
    {
      "Type": "Tree",
      "Position": [ 2, 0, -2 ],
      "Scale": [ 0.8, 0.8, 0.8 ]
    },
    {
      "Type": "Tree",
      "Position": [ 0, 0, -1 ],
      "Scale": [ 0.9, 0.9, 0.9 ]
    },
    {
      "Type": "Tree",
      "Position": [ -1, 0, -3 ],
      "Scale": [ 0.7, 0.7, 0.7 ]
    },
    {
      "Type": "Tree",
      "Position": [ 1, 0, -3 ],
      "Scale": [ 0.7, 0.7, 0.7 ]
    },
    {
      "Type": "Tree",
      "Position": [ -3, 0, -4 ],
      "Scale": [ 0.6, 0.6, 0.6 ]
    },
    {
      "Type": "Tree",
      "Position": [ 3, 0, -4 ],
      "Scale": [ 0.6, 0.6, 0.6 ]
    },
    {
      "Type": "Tree",
      "Position": [ 0, 0, -5 ],
      "Scale": [ 0.5, 0.5, 0.5 ]
    },
    {
      "Type": "Tree",
      "Position": [ -2, 0, -6 ],
      "Scale": [ 0.65, 0.65, 0.65 ]
    },
    {
      "Type": "Tree",
      "Position": [ 2, 0, -6 ],
      "Scale": [ 0.65, 0.65, 0.65 ]
    },
    {
      "Type": "Tree",
      "Position": [ -3, 0, -7 ],
      "Scale": [ 0.75, 0.75, 0.75 ]
    },
    {
      "Type": "Tree",
      "Position": [ 3, 0, -7 ],
      "Scale": [ 0.75, 0.75, 0.75 ]
    },
    {
      "Type": "Tree",
      "Position": [ 0, 0, -8 ],
      "Scale": [ 0.85, 0.85, 0.85 ]
    },
    {
      "Type": "YellowSphere",
      "Position": [ 0.5, 1.3, 0 ],
      "Scale": [ 0.1, 0.1, 0.1 ],
      "Rotation": [ 0, 0, 1 ]
    },
    {
      "Type": "YellowSphere",
      "Position": [ -1, 1.2, 0 ],
      "Scale": [ 0.1, 0.1, 0.1 ],
      "Rotation": [ 0, 0, 1 ]
    },
    {
      "Type": "GlassCube",
      "Position": [ 2, -0.5, -0.2 ],
      "Scale": [ 0.4, 0.4, 0.4 ],
      "Rotation": [ 0, 0, 1 ]
    },
    {
      "Type": "Floor",
      "Position": [ 0, -1, 0 ]
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


### More Images
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/53346e28-dd0a-449c-aa9a-4d7604b81eec)
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/921fc8c6-2bf0-4a01-89be-77cbc9a3d7bc)
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/6ada726b-eeb6-4041-bef3-f3bb80fdf037)
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/9ea9a333-0189-4437-9f03-1212b011f708)
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/d10b1832-286a-415f-a4d4-4ea08c87ba35)
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/103974f3-370c-4aa9-a307-5f8460044a0d)


#### Images of ifferent aliasing techniques:
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/9c820704-903d-4baa-9ba2-9837760504bf)*Hammersley antialising*
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/2f53ea77-095c-4ec4-aec2-3b5ffe799752)*Correlated Multi-Jittered antialiasing*
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/023f98da-e5b3-470d-a895-c7cf7050c451)*Jitteredsampling antialiasing*
![image](https://github.com/susenecka44/FotorealistickaGrafika/assets/97854742/a23c00c4-7b1c-48b5-9b40-d8d9b187a3a9)*Supersampling antialiasing*

### Use of AI

AI used for mostly fixing parts of code - debugging and small help.

Checkpoint IV.
- https://chat.openai.com/share/3253c7c8-56ea-4006-9030-ae4d3c33d87b
- https://chat.openai.com/share/816487f7-756d-4939-89ea-ff8c373b55ec

Checkpoint III.
- https://chat.openai.com/share/f69f1015-9448-4923-be62-176c46bcb8c8
- https://chat.openai.com/share/af8f2f21-3dc3-4ae0-be45-cb0791f8a29c
- https://chat.openai.com/share/7a216118-ae1c-43f5-8079-bb3a7cce1a35
- https://chat.openai.com/share/8c335fb4-eeb5-4717-b9da-51d143d67c62
