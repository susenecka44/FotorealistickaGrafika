# Sample scene description - raytracing "frustum"

This scene shows extreme distortion of 120-degree horizontal
view angle.

## Algorithm
* shadows = true
* reflections = true
* samplesPerPixel = 1
* maxDepth = 8
* minPerformance = 0.01

## Background
* color = [0.05, 0.1, 0.2]

## Camera
* width  = 600
* height = 450
* center = {0.00, 0.70,-5.00}
* dir    = {0.00,-0.15, 1.00}
* up     = {0.00, 1.00, 0.00}
* hAngle = 120 degrees

## Light sources
* ambient source
  * intensity = [1.0, 1.0, 1.0]
* point source
  * position  = {-10.0, 8.0,-8.0}
  * intensity = [1.0, 1.0, 1.0]

## Global BRDF
* Phong

## Materials
* phong material "yellow"
  * kA = 0.1
  * kD = 0.9
  * kS = 0.1
  * highlight = 8
  * color = [0.9, 0.8, 0.1]
* phong material "blue"
  * kA = 0.1
  * kD = 0.9
  * kS = 0.1
  * highlight = 8
  * color = [0.1, 0.1, 0.9]
* phong material "red"
  * kA = 0.1
  * kD = 0.9
  * kS = 0.1
  * highlight = 8
  * color = [0.9, 0.1, 0.1]
* phong material "green"
  * kA = 0.1
  * kD = 0.9
  * kS = 0.1
  * highlight = 8
  * color = [0.0, 0.8, 0.1]
* phong material "white"
  * kA = 0.1
  * kD = 0.9
  * kS = 0.1
  * highlight = 8
  * color = [0.9, 0.9, 0.9]

## Solids
* sphere
  * center = {-7.0, 4.0, 0.0}
  * radius = 0.6
  * material = yellow
* sphere
  * center = {0.0, 4.0, 0.0}
  * radius = 0.6
  * material = blue
* sphere
  * center = {7.0, 4.0, 0.0}
  * radius = 0.6
  * material = red
* sphere
  * center = {-7.0, 0.0, 0.0}
  * radius = 0.6
  * material = green
* sphere
  * center = {0.0, 0.0, 0.0}
  * radius = 0.6
  * material = yellow
* sphere
  * center = {7.0, 0.0, 0.0}
  * radius = 0.6
  * material = blue
* sphere
  * center = {-7.0, -4.0, 0.0}
  * radius = 0.6
  * material = red
* sphere
  * center = {0.0, -4.0, 0.0}
  * radius = 0.6
  * material = green
* sphere
  * center = {7.0, -4.0, 0.0}
  * radius = 0.6
  * material = yellow
* plane
  * origin = {0.0, -6.0, 0.0}
  * normal = {0.0, 1.0, 0.0}
  * material = white
