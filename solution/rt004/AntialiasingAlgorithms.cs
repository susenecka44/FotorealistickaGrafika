using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using System;

interface IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3 color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources);

}
public class RandomAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3 color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        Random random = new Random();

        for (int s = 0; s < algorithmSettings.SamplesPerPixel; ++s)
        {
            // Apply jittering: add a small random amount to each u and v coordinate
            float u = ((float)i + (float)random.NextDouble()) / (width - 1);
            float v = ((float)j + (float)random.NextDouble()) / (height - 1);

            Ray r = camera.GetRay(u, v);
            color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        }

        // Average the accumulated color by the number of samples and convert to [0, 1] range
        color /= algorithmSettings.SamplesPerPixel;
    }
}
public class NoAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3 color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        float u = (float)i / (width - 1);
        float v = (float)j / (height - 1);
        Ray r = camera.GetRay(u, v);
        color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
    }

}