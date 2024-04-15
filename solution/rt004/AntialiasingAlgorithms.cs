using CommandLine;
using System.Text.Json;
using System.Numerics;
using Util;
using System.Drawing;
using System;
using OpenTK.Mathematics;


interface IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources);

}
public class JitteredSamplingAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
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
    }
}

public class NoAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        double u = (double)i / (width - 1);
        double v = (double)j / (height - 1);
        Ray r = camera.GetRay(u, v);
        color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
    }

}

public class SupersamplingAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
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
    }
}

public class HammersleyAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
    {
        for (int s = 0; s < algorithmSettings.SamplesPerPixel; s++)
        {
            double u = (i + Hammersley(s, algorithmSettings.SamplesPerPixel)) / (width - 1);
            double v = (j + (double)s / algorithmSettings.SamplesPerPixel) / (height - 1);
            Ray r = camera.GetRay(u, v);
            color += raytracer.TraceRay(r, scene, lightSources, algorithmSettings.MaxDepth);
        }

        color /= algorithmSettings.SamplesPerPixel;
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

public class CorrelatedMultiJitteredAliasing : IAliasingAlgorithm
{
    public void PixelAlias(ref Vector3d color, ref int i, ref int j, ref int width, ref int height, ref ICamera camera, ref IRayTracer raytracer, ref AlgorithmSettings algorithmSettings, ref List<IHittable> scene, ref List<LightSource> lightSources)
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
    }
}
