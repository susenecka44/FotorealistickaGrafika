# Sample scene description

## Background
* color = [0.1, 0.2, 0.3]

## Camera
* width  = 600
* height = 450
* center = {0.60, 0.00,-5.60}
* dir    = {0.00,-0.03, 1.00}
* hAngle = 40 degrees

## Light sources
* ambient source
  * intensity = [1.0, 1.0, 1.0]
* point source
  * position  = {-10.0, 8.0,-6.0}
  * intensity = [1.0, 1.0, 1.0]
* point source
  * position  = {0.0, 20.0,-3.0}
  * intensity = [0.3,  0.3, 0.3]

## Global BRDF
* Phong

## Materials
* phong material "yellow"
  * kA = 0.1
  * kD = 0.8
  * kS = 0.2
  * highlight = 10
  * color = [1.0, 1.0, 0.2]
* phong material "blue"
  * kA = 0.1
  * kD = 0.5
  * kS = 0.5
  * highlight = 150
  * color = [0.2, 0.3, 1.0]
* phong material "red"
  * kA = 0.1
  * kD = 0.6
  * kS = 0.4
  * highlight = 80
  * color = [0.8, 0.2, 0.2]

## Solids
* sphere
  * center = {0.0, 0.0, 0.0}
  * radius = 1.0
  * material = yellow
* sphere
  * center = {1.4, -0.7, -0.5}
  * radius = 0.6
  * material = blue
* sphere
  * center = {-0.7, 0.7, -0.8}
  * radius = 0.1
  * material = red
