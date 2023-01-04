
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System;

namespace NebulaGen
{

    public enum FBMNoiseMode
    {
        Default = 0,
        Turbulence = 1,
        Ridges = 2,
        Inverted = 3,
    }

    public enum NoiseType
    {
        Perlin1,
        Perlin2,
        Worley1,
        Worley2,
        Simplex,
        Voronoi1,
        Voronoi2,
    }

    public enum NormalizationType
    {
        Stretch, // stretch min to 0 and max to 1
        Truncate, // value < min => 0, value > max => 1
    }

    [BurstCompile]
    public struct NativePalette
    {
        [ReadOnly] public NativeArray<float4> colors;
        [ReadOnly] public NativeArray<float> percentages;

        public NativePalette(int length, Allocator allocationType)
        {
            colors = new NativeArray<float4>(length, allocationType);
            percentages = new NativeArray<float>(length, allocationType);
        }

        public void Dispose()
        {
            colors.Dispose();
            percentages.Dispose();
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct NoiseOptions
    {
        [UnityEngine.SerializeField] public NoiseType noiseType;
        [UnityEngine.SerializeField] public FBMNoiseMode noiseMode;
        [UnityEngine.SerializeField] public NormalizationType normType;
        [UnityEngine.SerializeField][UnityEngine.Range(0, 1)] public float minCutoff;
        [UnityEngine.SerializeField][UnityEngine.Range(0, 1)] public float maxCutoff;
        [UnityEngine.Space]
        [UnityEngine.Space]
        [UnityEngine.SerializeField][UnityEngine.Range(0.01f, 5f)] public float perlinFactor;
        [UnityEngine.SerializeField] public float2 perlinOffset;
        [UnityEngine.SerializeField] public float voronoiAngleOffset;
        [UnityEngine.SerializeField] public float voronoiCellDensity;
        [UnityEngine.Space]
        [UnityEngine.Space]
        [UnityEngine.SerializeField][UnityEngine.Range(1, 12)] public int octaves;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 2f)] public float initialAmp;
        [UnityEngine.SerializeField][UnityEngine.Range(0.01f, 10f)] public float initialFreq;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 5f)] public float lacunarity;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 5f)] public float persistence;
        [UnityEngine.Space]
        [UnityEngine.Space]
        [UnityEngine.SerializeField][UnityEngine.Range(0, 2)] public int domainShiftPasses;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 200f)] public float domainShiftAmount;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 1f)] public float swirlAmount;
        [UnityEngine.SerializeField][UnityEngine.Range(1f, 50f)] public float swirlIntensity;
        [UnityEngine.SerializeField][UnityEngine.Range(0f, 1f)] public float warpAmount;
        [UnityEngine.SerializeField][UnityEngine.Range(1f, 50f)] public float warpIntensity;

        [UnityEngine.HideInInspector] public float mixAmount;
    }

    [BurstCompile]
    public struct JobProps
    {
        public int width;
        public int height;
        public float tilingFill;
    }

    public static class NebulaJobs
    {
        const float PASS_OFFSET_X_0 = 0f; const float PASS_OFFSET_Y_0 = 0f;
        const float PASS_OFFSET_X_1 = 5.2f; const float PASS_OFFSET_Y_1 = 1.3f;
        const float PASS_OFFSET_X_2 = 1.7f; const float PASS_OFFSET_Y_2 = 9.2f;
        const float PASS_OFFSET_X_3 = 8.3f; const float PASS_OFFSET_Y_3 = 2.8f;
        const float VORONOI_FREQ_MOD = 0.1f;

        [BurstCompile]
        public struct CalcCompositeMask : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float> mask1;
            [ReadOnly] public NativeArray<float> mask2;
            [ReadOnly] public float maskSelectPoint;
            [ReadOnly] public float maskFalloff;

            public NativeArray<float> mask;

            public void Execute(int current)
            {
                mask[current] = MaskSelect(0f, mask1[current], mask2[current], maskSelectPoint, maskFalloff);
            }
        }

        [BurstCompile]
        public struct CalcCompositeNoise : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float> noiseA;
            [ReadOnly] public NativeArray<float> noiseB;
            // [ReadOnly] public NativeArray<float> noiseC;

            public NativeArray<float> noise;

            public void Execute(int current)
            {
                // noise[current] = noiseA[current] + noiseB[current] + noiseC[current];
                noise[current] = noiseA[current] + noiseB[current];
            }
        }

        [BurstCompile]
        public static float MaskSelect(float a = 0f, float b = 0f, float selector = 0f, float selectPoint = 0f, float falloff = 0f)
        {
            // THE FOLLOWING CODE CHANGED TO A SELECT STATEMENT FOR BURST OPTIMIZATION
            // if (s >= selectPoint) return b;
            // if (s <= selectPoint - falloff) return a;
            // return math.lerp(a, b, 1.0f / ((selectPoint - (selectPoint - falloff)) / (selectPoint - s)));
            return math.select(
                math.select(
                    math.lerp(
                        a,
                        b,
                        math.clamp(1.0f / ((selectPoint - (selectPoint - falloff)) / (selectPoint - selector)), 0, 1)
                    ),
                    a,
                    selector <= selectPoint - falloff
                ),
                b,
                selector >= selectPoint
            );
        }

        [BurstCompile]
        public struct CalcColors : IJobParallelFor
        {

            [ReadOnly] public JobProps props;
            [ReadOnly] public ColorMode colorMode;
            [ReadOnly] public NativeArray<float> noise;
            [ReadOnly] public NativeArray<float2> colorLerps;
            [ReadOnly] public NativePalette paletteMain;
            [ReadOnly] public NativePalette paletteHighlight1;
            [ReadOnly] public NativePalette paletteHighlight2;
            [ReadOnly] public int enableDithering;
            [ReadOnly] public float ditherThreshold;
            [ReadOnly] public float blackPoint;
            [ReadOnly] public float highlight1;
            [ReadOnly] public float highlight2;
            [ReadOnly] public int noiseWidth;
            [ReadOnly] public int noiseHeight;

            public NativeArray<float4> pixels;

            public void Execute(int i)
            {
                float val = getValFromNoise(i);
                int ditherIndex = (i + i / props.width) * enableDithering * (colorMode == ColorMode.PixelArt ? 1 : 0);
                ditherThreshold = (colorMode == ColorMode.PixelArt ? ditherThreshold : 0f);
                pixels[i] = new float4(0f, 0f, 0f, 0f);
                if (val <= blackPoint) return;
                if (colorMode == ColorMode.PixelArt)
                {
                    pixels[i] = GetColorByValue(ref paletteMain, val, ditherIndex, ditherThreshold);
                    bool isHighlight1 = ((int)math.round(math.lerp(0f, colorLerps[i].x * 0.5f + 0.5f, highlight1 * colorLerps[i].x))) > 0;
                    bool isHighlight2 = ((int)math.round(math.lerp(0f, colorLerps[i].x * 0.5f + 0.5f, highlight2 * colorLerps[i].x))) > 0;
                    pixels[i] = isHighlight1 ? GetColorByValue(ref paletteHighlight1, val * colorLerps[i].x, ditherIndex, ditherThreshold) : pixels[i];
                    pixels[i] = isHighlight2 ? GetColorByValue(ref paletteHighlight2, val * colorLerps[i].y, ditherIndex, ditherThreshold) : pixels[i];
                }
                if (colorMode == ColorMode.RGB)
                {
                    pixels[i] = GetLerpedColorByValue(ref paletteMain, val);
                    pixels[i] = math.lerp(pixels[i], GetLerpedColorByValue(ref paletteHighlight1, val), math.lerp(0f, colorLerps[i].x, highlight1));
                    pixels[i] = math.lerp(pixels[i], GetLerpedColorByValue(ref paletteHighlight2, val), math.lerp(0f, colorLerps[i].y, highlight2));
                }
                // note - alpha will be set on final pass
            }

            float getValFromNoise(int current)
            {
                return noise[current];

                // get x, y for current pixel index
                int x = current % props.width;
                int y = current / props.width; // integer division
                // interpolate between floor and ceiling if noise grid size does not match output texture size
                float2 resample;
                resample.x = x * (float)noiseWidth / props.width;
                resample.y = y * (float)noiseHeight / props.height;
                float2 lower = math.clamp(math.floor(resample), new float2(0, 0), new float2(noiseWidth - 1, noiseHeight - 1));
                float2 upper = math.clamp(math.ceil(resample), new float2(0, 0), new float2(noiseWidth - 1, noiseHeight - 1));
                float noiseLower = noise[GetNoiseIndexFromCoords(lower)];
                float noiseUpper = noise[GetNoiseIndexFromCoords(upper)];
                float tx = (resample.x - lower.x) / (upper.x - lower.x);
                float ty = (resample.y - lower.y) / (upper.y - lower.y);
                float t = math.clamp((tx + ty) * 0.5f, 0, 1);
                return math.lerp(noiseLower, noiseUpper, t);
            }

            int GetNoiseIndexFromCoords(int x, int y)
            {
                return math.clamp(x, 0, noiseWidth - 1) + noiseWidth * math.clamp(y, 0, noiseHeight - 1);
            }

            int GetNoiseIndexFromCoords(float2 coords)
            {
                return GetNoiseIndexFromCoords((int)coords.x, (int)coords.y);
            }

            // given a value between 0-1, return corresponding palette color
            float4 GetColorByValue(ref NativePalette palette, float value, int pixelIndex = 0, float ditherThreshold = 0f)
            {
                if (value > 1) return palette.colors[palette.colors.Length - 1];
                float acc = 0f;
                float accPrev = 0f;
                for (int i = 0; i < palette.percentages.Length - 1; i++)
                {
                    accPrev = acc;
                    acc += palette.percentages[i];
                    if (value >= acc) continue;
                    float diffToPrev = (value - accPrev) * palette.percentages.Length;
                    float diffToNext = (acc - value) * palette.percentages.Length;
                    bool canDitherDown = ditherThreshold > diffToPrev
                        && pixelIndex % 2 == 0
                        && (diffToPrev < 0.2f || pixelIndex % 4 == 0)
                        && (diffToPrev < 0.4f || pixelIndex % 6 == 0);
                    bool canDitherUp = ditherThreshold > diffToNext
                        && pixelIndex % 2 == 1
                        && i < palette.percentages.Length - 1
                        && (diffToNext < 0.2f || pixelIndex % 4 == 1)
                        && (diffToNext < 0.4f || pixelIndex % 6 == 1);
                    if (canDitherDown) return i > 0 ? palette.colors[i - 1] : new float4(0f, 0f, 0f, 0f);
                    if (canDitherUp) return palette.colors[i + 1];
                    return palette.colors[i];
                }
                return palette.colors[palette.colors.Length - 1];
            }

            float4 GetLerpedColorByValue(ref NativePalette palette, float value)
            {
                if (value == 1) return palette.colors[palette.colors.Length - 1];
                return math.lerp(
                    palette.colors[GetClampedIndex(value * palette.percentages.Length, palette.percentages.Length)],
                    palette.colors[GetClampedIndex(value * palette.percentages.Length + 1, palette.percentages.Length)],
                    value * palette.percentages.Length % 1
                );
            }

            int GetClampedIndex(float index, int length)
            {
                return math.clamp((int)math.floor(index), 0, length - 1);
            }
        }

        [BurstCompile]
        public struct CalcFinalColorPass : IJobParallelFor
        {
            [ReadOnly] public float contrast;
            [ReadOnly] public float brightness;
            [ReadOnly] public float minAlpha;
            [ReadOnly] public float maxAlpha;

            public NativeArray<float4> pixels;

            public void Execute(int i)
            {
                float val = GetColorValue(pixels[i]);
                float adjusted = val > 0.01f ? math.pow(val, contrast) * brightness : 0f;
                float4 newColor = pixels[i];
                newColor.x *= adjusted;
                newColor.y *= adjusted;
                newColor.z *= adjusted;
                newColor.w = val > 0.01f ? math.lerp(minAlpha, maxAlpha, val) : 0f;
                pixels[i] = newColor;
            }

            float GetColorValue(float4 color)
            {
                return math.max(color.x, math.max(color.y, color.z));
            }
        }

        [BurstCompile]
        public struct CalcNoise : IJobParallelFor
        {
            [ReadOnly] public NoiseOptions options;
            [ReadOnly] public JobProps props;

            public NativeArray<float> noise;

            public void Execute(int current)
            {
                int x = current % props.width;
                int y = current / props.width; // integer division
                (float value, float2 colorLerp) = GetFBMDomainShifted(x, y, props.width, props.height, options);
                noise[current] = value * options.mixAmount;
            }
        }

        [BurstCompile]
        public struct CalcNoiseWithColorLerps : IJobParallelFor
        {
            [ReadOnly] public NoiseOptions options;
            [ReadOnly] public JobProps props;

            public NativeArray<float> noise;
            public NativeArray<float2> colorLerps;

            public void Execute(int current)
            {
                int x = current % props.width;
                int y = current / props.width; // integer division
                (float value, float2 colorLerp) = GetFBMDomainShifted(x, y, props.width, props.height, options);
                noise[current] = value * options.mixAmount;

                // mix in horizontally, vertically shifted values at edges for seamless tiling
                (float valueTileH, float2 _) = GetFBMDomainShifted(x + props.width, y, props.width, props.height, options);
                (float valueTileV, float2 _) = GetFBMDomainShifted(x, y + props.height, props.width, props.height, options);
                float th = math.max(props.tilingFill - x, 0) / math.max(props.tilingFill, 1);
                float tv = math.max(props.tilingFill - y, 0) / math.max(props.tilingFill, 1);
                noise[current] = math.lerp(noise[current], valueTileH, math.clamp(th, 0, 1));
                noise[current] = math.lerp(noise[current], valueTileV, math.clamp(tv, 0, 1));

                colorLerps[current] = colorLerp * options.mixAmount;
            }
        }

        [BurstCompile]
        public struct NormalizeNoise : IJob
        {
            [ReadOnly] public NoiseOptions options;

            public NativeArray<float> noise;

            public void Execute()
            {
                float min = 1f, max = 0f;
                float turbulence = 0f, ridges = 0f, inverted = 0f;

                float minThreshold = math.select(0f, options.minCutoff, options.normType == NormalizationType.Stretch);
                float maxThreshold = math.select(1f, options.maxCutoff, options.normType == NormalizationType.Stretch);
                for (int i = 0; i < noise.Length; i++)
                {
                    max = math.max(max, noise[i]);
                    min = math.min(min, noise[i]);
                }
                for (int i = 0; i < noise.Length; i++)
                {
                    noise[i] = math.lerp(0f, 1f, math.clamp(math.unlerp(min, max, noise[i]), 0, 1));
                    // clip noise below min threshold
                    noise[i] = math.select(noise[i], minThreshold, noise[i] < options.minCutoff);
                    // clip noise above max threshold
                    noise[i] = math.select(noise[i], maxThreshold, noise[i] > options.maxCutoff);
                    // stretch values to 0-1, or truncate based on normalization type (already truncated so do nothing)
                    noise[i] = math.select(
                        noise[i],
                        math.unlerp(options.minCutoff, options.maxCutoff, noise[i]),
                        options.normType == NormalizationType.Stretch
                    );
                    // calc different noise modes
                    turbulence = math.abs(math.lerp(-1f, 1f, math.clamp(noise[i], 0, 1)));
                    ridges = 1 - turbulence;
                    ridges *= ridges;
                    inverted = 1 - noise[i];
                    inverted *= inverted;
                    // this monstrosity
                    noise[i] = math.select(
                        math.select(
                            math.select(
                                noise[i],
                                ridges,
                                options.noiseMode == FBMNoiseMode.Ridges
                            ),
                            turbulence,
                            options.noiseMode == FBMNoiseMode.Turbulence
                        ),
                        inverted,
                        options.noiseMode == FBMNoiseMode.Inverted
                    );
                }
            }
        }

        [BurstCompile]
        public struct NormalizeColorLerps : IJob
        {
            public NativeArray<float2> colorLerps;

            public void Execute()
            {
                float2 maxValue = 0f;
                float2 minValue = 1f;
                for (int i = 0; i < colorLerps.Length; i++)
                {
                    maxValue = math.max(maxValue, colorLerps[i]);
                    minValue = math.min(minValue, colorLerps[i]);
                }
                for (int i = 0; i < colorLerps.Length; i++)
                {
                    colorLerps[i] = new float2(
                        math.lerp(minValue.x, maxValue.x, math.clamp(math.unlerp(minValue.x, maxValue.x, colorLerps[i].x), 0, 1)),
                        math.lerp(minValue.y, maxValue.y, math.clamp(math.unlerp(minValue.y, maxValue.y, colorLerps[i].y), 0, 1))
                    );
                }
            }
        }

        [BurstCompile]
        public struct CalcFalloffJob : IJobParallelFor
        {
            [ReadOnly] public JobProps props;
            [ReadOnly] public float amountBox;
            [ReadOnly] public float amountCircle;
            [ReadOnly] public float edgeDistance;
            [ReadOnly] public float edgeFalloff;
            [ReadOnly] public float edgeVarianceFactor;
            [ReadOnly] public float edgeVarianceStrength;

            public NativeArray<float> falloff;

            public void Execute(int current)
            {
                float distanceToEdge = math.max(float.MaxValue - float.MaxValue * amountBox - float.MaxValue * amountCircle, 0);
                distanceToEdge += getDistanceBox(current) * amountBox;
                distanceToEdge += getDistanceCircle(current) * amountCircle;
                distanceToEdge += getVariance(current, edgeVarianceFactor, edgeVarianceStrength, props);
                falloff[current] = math.lerp(0f, 1f, math.clamp((distanceToEdge - edgeDistance) / edgeFalloff, 0, 1));
            }

            float getVariance(int current, float edgeVarianceFactor, float edgeVarianceStrength, JobProps props)
            {
                int x = current % props.width;
                int y = current / props.width;
                return GetNoise(x, y, 1f - edgeVarianceFactor, new NoiseOptions
                {
                    noiseType = NoiseType.Perlin1,
                    perlinFactor = 0.01f,
                    perlinOffset = new float2(100f, 100f),
                }) * -edgeVarianceStrength;
            }

            float getDistanceBox(int current)
            {
                int width = props.width;
                int height = props.height;
                float distToEdgeLeft = current % width;
                float distToEdgeRight = width - current % width;
                float distToEdgeTop = height - current / width;
                float distToEdgeBottom = current / width;
                return math.min(distToEdgeLeft,
                       math.min(distToEdgeRight,
                       math.min(distToEdgeTop, distToEdgeBottom)));
            }

            float getDistanceCircle(int current)
            {
                int width = props.width;
                int height = props.height;
                float maxDistance = math.min(width * 0.5f, height * 0.5f);
                // get x and y with center as (0,0)
                int x = (int)(current % width - width * 0.5f);
                int y = (int)(current / width - height * 0.5f); // integer division
                float distanceToEdge = maxDistance - math.sqrt(x * x + y * y); // pythagorean theorum
                return math.max(0f, distanceToEdge);
            }
        }

        [BurstCompile]
        public struct CalcTiling : IJob
        {
            [ReadOnly] public JobProps props;
            [ReadOnly] public float tilingFill;
            [ReadOnly] public float amountMirror;
            [ReadOnly] public float amountStretch;

            public NativeArray<float> noise;
            public NativeArray<float2> colorLerps;

            public void Execute()
            {
                int width = props.width;
                int height = props.height;

                float noisePrev = 0f, stepPercentage = 0f;
                int iCurrent, iCompare, iCompareStep, iMedian, numSteps;

                // work from bottom edge; compare against top edge
                for (int x = 0; x < width; x++)
                {
                    iCompare = GetIndexFromCoords(x, height - 1);
                    // get pixel from center row (mirror x) to add variance
                    iMedian = GetIndexFromCoords((width - 1) - x, (int)(height * 0.5f));
                    numSteps = (int)Max(tilingFill * noise[iMedian], tilingFill * noise[iCompare], 3);
                    if (math.abs(noise[x] - noise[iCompare]) < 0.01f) continue;
                    for (int y = 0; y <= numSteps; y++)
                    {
                        if (y == height) break;
                        stepPercentage = (float)y / numSteps;
                        iCurrent = GetIndexFromCoords(x, y);
                        iCompareStep = amountMirror == 1
                            ? GetIndexFromCoords(x, height - 1 - y)
                            : iCompare;
                        noisePrev = noise[iCurrent];
                        noise[iCurrent] = Lerp(noise[iCompareStep], noise[iCurrent], stepPercentage);
                        colorLerps[iCurrent] = new float2(
                            x: Lerp(colorLerps[iCompareStep].x, colorLerps[iCurrent].x, stepPercentage),
                            y: Lerp(colorLerps[iCompareStep].y, colorLerps[iCurrent].y, stepPercentage)
                        );
                        if (math.abs(noise[iCurrent] - noisePrev) < 0.01f) break;
                    }
                }

                // work from left edge; compare against right edge
                for (int y = 0; y < height; y++)
                {
                    iCurrent = GetIndexFromCoords(0, y);
                    iCompare = GetIndexFromCoords(width - 1, y);
                    // get pixel from center column (mirror y) to add variance
                    iMedian = GetIndexFromCoords((int)(width * 0.5f), (height - 1) - y);
                    numSteps = (int)Max(tilingFill * noise[iMedian], tilingFill * noise[iCompare], 3);
                    if (math.abs(noise[iCurrent] - noise[iCompare]) < 0.01f) continue;
                    for (int x = 0; x <= numSteps; x++)
                    {
                        if (x == width) break;
                        stepPercentage = (float)x / numSteps;
                        iCurrent = GetIndexFromCoords(x, y);
                        iCompareStep = amountMirror == 1
                            ? GetIndexFromCoords(width - 1 - x, y)
                            : iCompare;
                        noisePrev = noise[iCurrent];
                        noise[iCurrent] = Lerp(noise[iCompareStep], noise[iCurrent], stepPercentage);
                        colorLerps[iCurrent] = new float2(
                            x: Lerp(colorLerps[iCompareStep].x, colorLerps[iCurrent].x, stepPercentage),
                            y: Lerp(colorLerps[iCompareStep].y, colorLerps[iCurrent].y, stepPercentage)
                        );
                        if (math.abs(noise[iCurrent] - noisePrev) < 0.01f) break;
                    }
                }
            }

            float Lerp(float a, float b, float t)
            {
                return math.lerp(a, b, math.clamp(t, 0f, 1f));
            }

            float Max(float a, float b, float c)
            {
                return math.max(a, math.max(b, c));
            }

            int GetIndexFromCoords(int x, int y)
            {
                return math.clamp(x, 0, props.width - 1) + props.width * math.clamp(y, 0, props.height - 1);
            }
        }

        // For domain shift algorithm explanation, see: https://iquilezles.org/articles/warp/
        [BurstCompile]
        static (float value, float2 colorLerp) GetFBMDomainShifted(int inputX, int inputY, int width, int height, NoiseOptions options)
        {
            float value = 0f;
            float2 colorLerp = new float2(0f, 0f);
            (float x, float y) = GetSwirl(inputX, inputY, width, height, options);
            (x, y) = GetGridWarp(x, y, width, height, options);
            if (options.domainShiftPasses == 0)
            {
                value = GetFBM(x, y, options);
            }
            else if (options.domainShiftPasses == 1)
            {
                float2 p = new float2(x, y);
                float2 q = new float2(
                    GetFBM(p, options),
                    GetFBM(p + GetPassOffset(0), options)
                );
                value = GetFBM(p + q * options.domainShiftAmount, options);
                colorLerp.x = math.length(q);
            }
            else if (options.domainShiftPasses == 2)
            {
                float2 p = new float2(x, y);
                float2 q = new float2(
                    GetFBM(p + GetPassOffset(0), options),
                    GetFBM(p + GetPassOffset(1), options)
                );
                float2 r = new float2(
                    GetFBM(p + q * options.domainShiftAmount + GetPassOffset(2), options),
                    GetFBM(p + q * options.domainShiftAmount + GetPassOffset(3), options)
                );
                value = GetFBM(p + r * options.domainShiftAmount, options);
                colorLerp.x = math.length(q);
                colorLerp.y = r.y;
            }
            return (value, colorLerp);
        }

        [BurstCompile]
        static (float x, float y) GetSwirl(float x, float y, float width, float height, NoiseOptions options)
        {
            if (options.swirlAmount <= 0f) return (x, y);
            float distX = (x - width * 0.5f);
            float distY = (y - height * 0.5f);
            float distanceToCenter = math.sqrt(distX * distX + distY * distY) / math.max(width * 0.5f, height * 0.5f);
            distX = math.lerp(distX, distX * (1f - distanceToCenter) * 10f, options.swirlAmount);
            distY = math.lerp(distY, distY * (1f - distanceToCenter) * 10f, options.swirlAmount);
            float2 offset = rotation(new float2(distX, distY), distanceToCenter * options.swirlAmount * options.swirlIntensity);
            return (
                x + offset.x,
                y + offset.y
            );
        }

        [BurstCompile]
        static (float x, float y) GetGridWarp(float x, float y, float width, float height, NoiseOptions options)
        {
            if (options.warpAmount <= 0f) return (x, y);
            float offsetX = (x - width * 0.5f) / (width * 0.5f);
            float offsetY = (y - height * 0.5f) / (height * 0.5f);
            float distanceToCenter = math.sqrt(offsetX * offsetX + offsetY * offsetY);
            offsetX = math.cos(offsetX * 2f * math.PI * options.warpAmount) * distanceToCenter * options.warpIntensity;
            offsetY = math.sin(offsetY * 2f * math.PI * options.warpAmount) * distanceToCenter * options.warpIntensity;
            return (
                x + offsetX,
                y + offsetY
            );
        }

        [BurstCompile]
        static float2 rotation(float2 p, float amount)
        {
            return math.mul(p, new float2x2(math.cos(amount), -math.sin(amount),
                                            math.sin(amount), math.cos(amount)));
        }

        [BurstCompile]
        static float GetFBM(float x, float y, NoiseOptions options)
        {
            float amplitude = options.initialAmp;
            float frequency = options.initialFreq * 0.01f;
            float sample = 0f;

            if (options.noiseType == NoiseType.Voronoi1 ||
                options.noiseType == NoiseType.Voronoi2)
            {
                amplitude *= 0.25f;
            }

            amplitude *= math.select(1f, 0.25f, options.noiseType == NoiseType.Voronoi1 || options.noiseType == NoiseType.Voronoi2);

            for (int oct = 0; oct < options.octaves; oct++)
            {
                sample += math.clamp(GetNoise(x, y, frequency, options), 0, 1) * amplitude;
                frequency *= options.lacunarity;
                amplitude *= options.persistence;
            }

            return sample;
        }

        [BurstCompile]
        static float GetFBM(float2 p, NoiseOptions options)
        {
            return GetFBM(p.x, p.y, options);
        }

        [BurstCompile]
        static float2 GetPassOffset(int index)
        {
            return math.select(
                math.select(
                    math.select(
                        new float2(PASS_OFFSET_X_3, PASS_OFFSET_Y_3),
                        new float2(PASS_OFFSET_X_2, PASS_OFFSET_Y_2),
                        index % 4 == 2
                    ),
                    new float2(PASS_OFFSET_X_1, PASS_OFFSET_Y_1),
                    index % 4 == 1
                ),
                new float2(PASS_OFFSET_X_0, PASS_OFFSET_Y_0),
                index % 4 == 0
            );
        }

        static float GetNoise(float x, float y, float frequency, NoiseOptions options)
        {
            float xCoord = options.perlinOffset.x + x * options.perlinFactor * frequency;
            float yCoord = options.perlinOffset.y + y * options.perlinFactor * frequency;

            switch (options.noiseType)
            {
                case NoiseType.Perlin1:
                default:
                    return UnityEngine.Mathf.PerlinNoise(xCoord, yCoord);
                case NoiseType.Perlin2:
                    return noise.cnoise(new float2(xCoord, yCoord));
                case NoiseType.Worley1:
                    return noise.cellular(new float2(xCoord, yCoord)).x;
                case NoiseType.Worley2:
                    return noise.cellular(new float2(xCoord, yCoord)).y * 0.2856423854f;
                case NoiseType.Simplex:
                    return noise.snoise(new float2(xCoord, yCoord));
                case NoiseType.Voronoi1:
                    return GetVoronoiNoise(new float2(xCoord * VORONOI_FREQ_MOD, yCoord * VORONOI_FREQ_MOD), options.voronoiAngleOffset, options.voronoiCellDensity).output;
                case NoiseType.Voronoi2:
                    return GetVoronoiNoise(new float2(xCoord * VORONOI_FREQ_MOD, yCoord * VORONOI_FREQ_MOD), options.voronoiAngleOffset, options.voronoiCellDensity).cells;
            }
        }

        [BurstCompile]
        static float2 unity_voronoi_noise_randomVector(float2 uv, float offset)
        {
            float2x2 m = new float2x2(15.27f, 47.63f, 99.41f, 89.98f);
            uv = math.frac(math.sin(math.mul(uv, m)) * 46839.32f);
            return new float2(math.sin(uv.y * offset) * 0.5f + 0.5f, math.cos(uv.x * offset) * 0.5f + 0.5f);
        }

        [BurstCompile]
        struct VoronoiValue
        {
            public float output;
            public float cells;
        }

        [BurstCompile]
        static VoronoiValue GetVoronoiNoise(float2 uv, float angleOffset, float cellDensity)
        {
            float2 g = math.floor(uv * cellDensity);
            float2 f = math.frac(uv * cellDensity);
            float3 res = new float3(8.0f, 0.0f, 0.0f);

            VoronoiValue value = new VoronoiValue();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = new float2(x, y);
                    float2 offset = unity_voronoi_noise_randomVector(lattice + g, angleOffset);
                    float d = math.distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = new float3(d, offset.x, offset.y);
                        value.output = res.x;
                        value.cells = res.y;
                    }
                }
            }
            return value;
        }
    }
}
