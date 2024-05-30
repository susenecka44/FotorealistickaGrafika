using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using System;
using OpenTK.Mathematics;

/// <summary>
/// Interface for all antialiasing algorithms
/// </summary>
interface IAliasingAlgorithm
{
    /// <summary>
    /// Anti-aliasing algorithm that returns the color of the pixel based on the algorithm
    /// </summary>
    /// <param name="color"> final color </param>
    /// <param name="i"> pixel coordinate </param>
    /// <param name="j"> pixel coordinate </param>
    /// <param name="width"> image width </param>
    /// <param name="height"> image height </param>
    /// <param name="camera"> camera used </param>
    /// <param name="raytracer"> raytracer used </param>
    /// <param name="algorithmSettings"> algorithm settings used </param>
    /// <param name="scene"> objects in scene </param>
    /// <param name="lightSources"> lights in scene </param>
    /// <returns></returns>
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources);

}

/// <summary>
/// Jittered sampling antialiasing algorithm
/// -> jittered sampling is a technique that randomly samples the pixel multiple times and averages the result
/// </summary>
public class JitteredSamplingAliasing : IAliasingAlgorithm
{
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        int sqrtSamples = (int)Math.Sqrt(algorithmSettings.SamplesPerPixel);
        Random random = new Random();

        for (int s = 0; s < sqrtSamples; ++s)
        {
            double u = (i + (s + random.NextDouble()) / sqrtSamples) / (width - 1);
            double v = (j + (s + random.NextDouble()) / sqrtSamples) / (height - 1);
            Ray r = camera.GetRay(u, v);
            color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        }
        color /= sqrtSamples;
        return color;

    }
}

/// <summary>
/// No aliasing algorithm
/// </summary>
public class NoAliasing : IAliasingAlgorithm
{
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        double u = (double)i / (width - 1);
        double v = (double)j / (height - 1);
        Ray r = camera.GetRay(u, v);
        color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        return color;
    }

}

/// <summary>
/// Supersampling antialiasing algorithm
/// -> supersampling is a technique that samples the pixel multiple times and averages the result
/// </summary>
public class SupersamplingAliasing : IAliasingAlgorithm
{
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        int sqrtSamples = (int)Math.Sqrt(algorithmSettings.SamplesPerPixel);
        Random random = new Random();

        for (int dx = 0; dx < sqrtSamples; dx++)
        {
            for (int dy = 0; dy < sqrtSamples; dy++)
            {
                double u = (i + (dx + random.NextDouble()) / sqrtSamples) / (width - 1);
                double v = (j + (dy + random.NextDouble()) / sqrtSamples) / (height - 1);
                Ray r = camera.GetRay(u, v);
                color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
            }
        }

        color /= (sqrtSamples * sqrtSamples);
        return color;
    }
}

/// <summary>
/// Hammersley aliasing algorithm
/// -> Hammersley is a technique that samples the pixel methodically multiple times and averages the result
/// </summary>
public class HammersleyAliasing : IAliasingAlgorithm
{
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        for (int s = 0; s < algorithmSettings.SamplesPerPixel; s++)
        {
            double u = (i + Hammersley(s, algorithmSettings.SamplesPerPixel)) / (width - 1);
            double v = (j + (double)s / algorithmSettings.SamplesPerPixel) / (height - 1);
            Ray r = camera.GetRay(u, v);
            color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        }

        color /= algorithmSettings.SamplesPerPixel;
        return color;
    }

    // the calculation based on the slides and wikipedia :]
    private double Hammersley(int index, int numSamples)
    {
        double result = 0;
        double f = 1.0;
        int i = index;
        while (i > 0)
        {
            f /= numSamples;
            result += f * (i % 2);
            i /= 2;
        }
        return result;
    }
}

/// <summary>
/// Correlated multi-jittered aliasing algorithm
/// -> Correlated multi-jittered is a technique that samples the pixel randomly multiple times and averages the result
/// </summary>
public class CorrelatedMultiJitteredAliasing : IAliasingAlgorithm
{
    public Vector3d PixelAlias(Vector3d color, int i, int j, int width, int height, ICamera camera, IRayTracer raytracer, AlgorithmSettings algorithmSettings, List<IHittable> scene, List<LightSource> lightSources)
    {
        int subPixelGrid = (int)Math.Sqrt(algorithmSettings.SamplesPerPixel);
        Random random = new Random();

        for (int m = 0; m < subPixelGrid; m++)
        {
            for (int n = 0; n < subPixelGrid; n++)
            {
                double sj = (n + (m + random.NextDouble()) / subPixelGrid) / subPixelGrid;
                double si = (m + (n + random.NextDouble()) / subPixelGrid) / subPixelGrid;
                double u = (i + si) / (width - 1);
                double v = (j + sj) / (height - 1);
                Ray r = camera.GetRay(u, v);
                color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
            }
        }

        color /= algorithmSettings.SamplesPerPixel;
        return color;
    }
}
