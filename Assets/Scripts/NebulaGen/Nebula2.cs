
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// TODO
// - [ ] get old project working
// - [ ] get min/max noise fnc working (add separate normalize passes)
// - [ ] add new fancy noise textures
// - [ ] add drawable masking
// - [ ] add camera controls (pan, zoom)
// - [ ] flesh out with scifi UI

namespace NebulaGen
{

    public enum ColorMode
    {
        PixelArt,
        RGB,
    }

    public class Nebula2 : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Header("Refs")]
        [SerializeField] RawImage outputImage;
        [SerializeField][Range(0f, 2f)] float repaintDelay = 0.2f;

        [Space]
        [Space]

        [Header("Dimensions")]
        [SerializeField][Range(32, 960)] int sizeX = 480;
        [SerializeField][Range(32, 960)] int sizeY = 480;
        [SerializeField][Range(1, 10)] int sizeMod = 1;

        [Space]
        [Space]

        [Header("Noise")]
        [SerializeField] bool debugCompositeNoise = false;
        [SerializeField]
        NoiseOptions noiseLayerA = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.3f,
            perlinOffset = Vector2.one * 15f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            // minCutoff = 0.2f,
            // maxCutoff = 0.8f,
            initialAmp = 1f,
            initialFreq = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
            domainShiftPasses = 0,
            domainShiftAmount = 10f,
            swirlAmount = 0f,
            swirlIntensity = 1f,
            warpAmount = 0f,
            warpIntensity = 1f,
        };
        [SerializeField]
        NoiseOptions noiseLayerB = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.4f,
            perlinOffset = Vector2.one * 25f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            // minCutoff = 0.2f,
            // maxCutoff = 1f,
            initialAmp = 1f,
            initialFreq = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
            domainShiftPasses = 0,
            domainShiftAmount = 10f,
            swirlAmount = 0f,
            swirlIntensity = 1f,
            warpAmount = 0f,
            warpIntensity = 1f,
        };
        [SerializeField]
        NoiseOptions noiseLayerC = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.4f,
            perlinOffset = Vector2.one * 25f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            // minCutoff = 0.2f,
            // maxCutoff = 1f,
            initialAmp = 1f,
            initialFreq = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
            domainShiftPasses = 0,
            domainShiftAmount = 10f,
            swirlAmount = 0f,
            swirlIntensity = 1f,
            warpAmount = 0f,
            warpIntensity = 1f,
        };

        [Header("Mask")]

        [SerializeField] bool debugMasks = false;
        [SerializeField][Range(0, 1)] float maskSelectPoint = 0.4f;
        [SerializeField][Range(0, 1)] float maskFalloff = 0.1f;
        [SerializeField]
        NoiseOptions maskLayerA = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = .3f,
            perlinOffset = Vector2.one * 5f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            // minCutoff = 0.2f,
            // maxCutoff = 0.8f,
            initialFreq = 1f,
            initialAmp = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
        };
        [SerializeField]
        NoiseOptions maskLayerB = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = .35f,
            perlinOffset = Vector2.one * 10f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            // minCutoff = 0.2f,
            // maxCutoff = 0.8f,
            initialFreq = 1f,
            initialAmp = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
        };

        [Space]
        [Space]

        [Header("Mix")]
        [SerializeField][Range(0f, 1f)] float mixNoiseA = 1f;
        [SerializeField][Range(0f, 1f)] float mixNoiseB = 0f;
        [SerializeField][Range(0f, 1f)] float mixNoiseC = 0F;
        [SerializeField][Range(0f, 1f)] float mixMask = 1f;
        [SerializeField] AnimationCurve outputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField][Range(0f, 1f)] float minCutoff = 0f;
        [SerializeField][Range(0f, 1f)] float maxCutoff = 0f;

        [Space]
        [Space]

        [Header("Colours")]
        [SerializeField] ColorMode colorMode;
        [SerializeField] ColorPalette.Palette paletteMain;
        [SerializeField] ColorPalette.Palette paletteHighlight1;
        [SerializeField] ColorPalette.Palette paletteHighlight2;
        [SerializeField][Range(0f, 1f)] float highlight1 = 1f;
        [SerializeField][Range(0f, 1f)] float highlight2 = 1f;
        [SerializeField][Range(0f, 1f)] float maxAlpha = 1f;
        [SerializeField][Range(0f, 1f)] float minAlpha = 0f;
        [SerializeField][Range(0f, 1f)] float brightness = 1f;
        [SerializeField][Range(1f, 10f)] float contrast = 1f;
        [SerializeField][Range(0f, 0.5f)] float blackPoint = 0.01f;

        [Space]
        [Space]

        [Header("Border")]
        [SerializeField] BorderMode borderMode = BorderMode.FalloffBox;
        [SerializeField][Range(0f, 50f)] float edgeDistance = 5f;
        [SerializeField][Range(0f, 1000f)] float edgeFalloff = 5f;
        [SerializeField][Range(0f, 10f)] float edgeVarianceFactor = 1f;
        [SerializeField][Range(0f, 150f)] float edgeVarianceStrength = 1f;
        [SerializeField][Range(0f, 100)] int tilingFill = 50;

        [Space]
        [Space]

        [Header("Pixel Art")]

        [SerializeField] bool enableDithering = true;
        [SerializeField][Range(0f, 1f)] float ditherThreshold = 0.25f;

        enum BorderMode
        {
            None,
            FalloffBox,
            FalloffCircle,
            TileMirror,
            TileStretch,
        }

        int width;
        int height;

        Color[] _pixels;
        float[] _noise;
        float[] _maskComposite;
        float2[] _colorLerps;
        bool shouldFalloff;
        NoiseOptions options = new NoiseOptions();

        bool shouldGenerate;
        float timeElapsedSinceGenerating = float.MaxValue;

        void Start()
        {
            shouldGenerate = true;
        }

        void Update()
        {
            if (!shouldGenerate) return;
            if (timeElapsedSinceGenerating < repaintDelay) return;
            shouldGenerate = false;
            Generate();
        }

        void LateUpdate()
        {
            timeElapsedSinceGenerating += Time.deltaTime;
        }

        void Generate()
        {
            Calculate();
            bool didChangeSize = outputImage.texture.width != width || outputImage.texture.height != height;
            if (didChangeSize || IsDebug())
            {
                Texture2D texture = GenerateTexture();
                outputImage.texture = texture;
                outputImage.SetNativeSize();
            }
            else
            {
                (outputImage.texture as Texture2D).SetPixels(_pixels);
                (outputImage.texture as Texture2D).Apply();
            }
            timeElapsedSinceGenerating = 0f;
        }

        bool IsDebug()
        {
            return false
                || debugMasks
                || debugCompositeNoise;
        }

        void Calculate()
        {
            width = sizeX * sizeMod;
            height = sizeY * sizeMod;
            CalcNoise();
            Assert.AreEqual(_noise.Length, width * height);
            CalcPixels();
            Assert.AreEqual(_pixels.Length, width * height);
        }

        Texture2D GenerateTexture()
        {
            if (debugMasks) return NoiseToTexture2D(_maskComposite);
            if (debugCompositeNoise) return NoiseToTexture2D(_noise);
            return ColorToTexture2D(_pixels);
        }

        void CalcNoise()
        {
            InitNoise(ref _noise);
            InitNoise(ref _maskComposite);
            InitColorLerps(ref _colorLerps);
            InitPixels(ref _pixels);

            JobProps props = new JobProps();
            props.width = width;
            props.height = height;
            int length = width * height;

            NativeArray<float> noise = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask = new NativeArray<float>(length, Allocator.TempJob);

            NativeArray<float> noiseA = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseB = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseC = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask1 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask2 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> falloff = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float2> colorLerps = new NativeArray<float2>(length, Allocator.TempJob);

            #region NOISE_CALC
            noiseLayerA.mixAmount = mixNoiseA;
            noiseLayerB.mixAmount = mixNoiseB;
            noiseLayerC.mixAmount = mixNoiseC;
            maskLayerA.mixAmount = 1f;
            maskLayerB.mixAmount = 1f;
            NebulaJobs.CalcNoiseWithColorLerps jobNoiseA = new NebulaJobs.CalcNoiseWithColorLerps
            {
                noise = noiseA,
                colorLerps = colorLerps,
                options = noiseLayerA,
                props = props,
            };
            NebulaJobs.CalcNoise jobNoiseB = new NebulaJobs.CalcNoise
            {
                noise = noiseB,
                options = noiseLayerB,
                props = props,
            };
            NebulaJobs.CalcNoise jobNoiseC = new NebulaJobs.CalcNoise
            {
                noise = noiseC,
                options = noiseLayerC,
                props = props,
            };
            NebulaJobs.CalcNoise jobMask1 = new NebulaJobs.CalcNoise
            {
                noise = mask1,
                options = maskLayerA,
                props = props,
            };
            NebulaJobs.CalcNoise jobMask2 = new NebulaJobs.CalcNoise
            {
                noise = mask2,
                options = maskLayerB,
                props = props,
            };
            NebulaJobs.CalcFalloffJob jobFalloff = new NebulaJobs.CalcFalloffJob
            {
                falloff = falloff,
                props = props,
                amountBox = borderMode == BorderMode.FalloffBox ? 1f : 0f,
                amountCircle = borderMode == BorderMode.FalloffCircle ? 1f : 0f,
                edgeDistance = edgeDistance,
                edgeFalloff = edgeFalloff,
                edgeVarianceFactor = edgeVarianceFactor,
                edgeVarianceStrength = edgeVarianceStrength,
            };
            JobHandle handleNoiseA = jobNoiseA.Schedule(length, 1);
            JobHandle handleNoiseB = jobNoiseB.Schedule(length, 1);
            JobHandle handleNoiseC = jobNoiseC.Schedule(length, 1);
            JobHandle handleMask1 = jobMask1.Schedule(length, 1);
            JobHandle handleMask2 = jobMask2.Schedule(length, 1);
            JobHandle handleFalloff = jobFalloff.Schedule(length, 1);

            handleNoiseA.Complete();
            handleNoiseB.Complete();
            handleNoiseC.Complete();
            handleMask1.Complete();
            handleMask2.Complete();
            handleFalloff.Complete();
            #endregion NOISE_CALC

            #region COMPOSITING
            NebulaJobs.CalcCompositeMask jobCompositeMask = new NebulaJobs.CalcCompositeMask
            {
                mask1 = mask1,
                mask2 = mask2,
                maskSelectPoint = maskSelectPoint,
                maskFalloff = maskFalloff,
                mask = mask,
            };
            NebulaJobs.CalcCompositeNoise jobCompositeNoise = new NebulaJobs.CalcCompositeNoise
            {
                noiseA = noiseA,
                noiseB = noiseB,
                noiseC = noiseC,
                noise = noise,
            };
            JobHandle handleCompositeMask = jobCompositeMask.Schedule(length, 1);
            JobHandle handleCompositeNoise = jobCompositeNoise.Schedule(length, 1);
            handleCompositeMask.Complete();
            handleCompositeNoise.Complete();
            #endregion COMPOSITING

            #region NORMALIZATION
            NebulaJobs.NormalizeNoise jobNormalizeNoise = new NebulaJobs.NormalizeNoise
            {
                noise = noise,
                minCutoff = minCutoff,
                maxCutoff = maxCutoff,
            };
            NebulaJobs.NormalizeNoise jobNormalizeMask = new NebulaJobs.NormalizeNoise
            {
                noise = mask,
                minCutoff = 0f,
                maxCutoff = 1f,
            };
            NebulaJobs.NormalizeColorLerps jobNormalizeColorLerps = new NebulaJobs.NormalizeColorLerps
            {
                colorLerps = colorLerps,
            };
            JobHandle handleNormalizeNoise = jobNormalizeNoise.Schedule();
            JobHandle handleNormalizeMask = jobNormalizeMask.Schedule();
            JobHandle handleNormalizeColorLerps = jobNormalizeColorLerps.Schedule();
            handleNormalizeNoise.Complete();
            handleNormalizeMask.Complete();
            handleNormalizeColorLerps.Complete();
            #endregion NORMALIZATION

            #region SUBTRACTION
            for (int i = 0; i < length; i++)
            {
                float mod = Mathf.Lerp(1f, mask[i], mixMask);
                float val = outputCurve.Evaluate(noise[i]) * mod * Mathf.Clamp01(falloff[i]);
                noise[i] = val <= blackPoint ? 0f : val;
            }
            #endregion SUBTRACTION

            #region TILING
            if (borderMode == BorderMode.TileMirror || borderMode == BorderMode.TileStretch)
            {
                NebulaJobs.CalcTiling jobCalcTiling = new NebulaJobs.CalcTiling
                {
                    noise = noise,
                    colorLerps = colorLerps,
                    props = props,
                    tilingFill = tilingFill,
                    amountMirror = borderMode == BorderMode.TileMirror ? 1 : 0,
                    amountStretch = borderMode == BorderMode.TileStretch ? 1 : 0,
                };
                JobHandle handleCalcTiling = jobCalcTiling.Schedule();
                handleCalcTiling.Complete();
            }
            #endregion TILING

            for (int i = 0; i < length; i++)
            {
                this._noise[i] = noise[i];
                this._maskComposite[i] = mask[i];
                this._colorLerps[i] = colorLerps[i];
            }

            noise.Dispose();
            mask.Dispose();
            noiseA.Dispose();
            noiseB.Dispose();
            noiseC.Dispose();
            mask1.Dispose();
            mask2.Dispose();
            falloff.Dispose();
            colorLerps.Dispose();
        }

        void CalcPixels()
        {
            int length = width * height;
            Assert.AreEqual(_pixels.Length, length);
            Assert.AreEqual(_noise.Length, length);
            JobProps props = new JobProps { width = width, height = height };

            NativeArray<float> jobNoise = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float2> jobColorLerps = new NativeArray<float2>(length, Allocator.TempJob);
            NativePalette jobPaletteMain = new NativePalette(paletteMain.GetNumColors(), Allocator.TempJob);
            NativePalette jobPaletteHighlight1 = new NativePalette(paletteHighlight1.GetNumColors(), Allocator.TempJob);
            NativePalette jobPaletteHighlight2 = new NativePalette(paletteHighlight2.GetNumColors(), Allocator.TempJob);
            NativeArray<float4> pixels = new NativeArray<float4>(length, Allocator.TempJob);

            for (int i = 0; i < paletteMain.GetNumColors(); i++)
            {
                jobPaletteMain.colors[i] = paletteMain.GetColorByIndex(i).ToFloat4();
                jobPaletteMain.percentages[i] = paletteMain.GetColorPercentageByIndex(i);
            }
            for (int i = 0; i < paletteHighlight1.GetNumColors(); i++)
            {
                jobPaletteHighlight1.colors[i] = paletteHighlight1.GetColorByIndex(i).ToFloat4();
                jobPaletteHighlight1.percentages[i] = paletteHighlight1.GetColorPercentageByIndex(i);
            }
            for (int i = 0; i < paletteHighlight2.GetNumColors(); i++)
            {
                jobPaletteHighlight2.colors[i] = paletteHighlight2.GetColorByIndex(i).ToFloat4();
                jobPaletteHighlight2.percentages[i] = paletteHighlight2.GetColorPercentageByIndex(i);
            }
            for (int i = 0; i < _noise.Length; i++)
            {
                jobNoise[i] = _noise[i];
                jobColorLerps[i] = _colorLerps[i];
            }

            NebulaJobs.CalcColors jobCalcColorsPixelArt = new NebulaJobs.CalcColors
            {
                colorMode = colorMode,
                props = props,
                noise = jobNoise,
                colorLerps = jobColorLerps,
                paletteMain = jobPaletteMain,
                paletteHighlight1 = jobPaletteHighlight1,
                paletteHighlight2 = jobPaletteHighlight2,
                enableDithering = (enableDithering ? 1 : 0),
                ditherThreshold = ditherThreshold,
                blackPoint = blackPoint,
                highlight1 = highlight1,
                highlight2 = highlight2,
                pixels = pixels,
            };
            JobHandle handleCalcColorsPixelArt = jobCalcColorsPixelArt.Schedule(length, 1);
            handleCalcColorsPixelArt.Complete();

            NebulaJobs.CalcFinalColorPass jobCalcFinalColorPass = new NebulaJobs.CalcFinalColorPass
            {
                contrast = contrast,
                brightness = brightness,
                minAlpha = minAlpha,
                maxAlpha = maxAlpha,
                pixels = pixels,
            };
            JobHandle handleCalcFinalColorPass = jobCalcFinalColorPass.Schedule(length, 1);
            handleCalcFinalColorPass.Complete();

            for (int i = 0; i < _noise.Length; i++)
            {
                this._pixels[i] = new Color(
                    pixels[i].x,
                    pixels[i].y,
                    pixels[i].z,
                    pixels[i].w
                );
            }

            jobNoise.Dispose();
            jobColorLerps.Dispose();
            jobPaletteMain.Dispose();
            jobPaletteHighlight1.Dispose();
            jobPaletteHighlight2.Dispose();
            pixels.Dispose();
        }

        void CalcFinalColorPass()
        {
            int length = width * height;
            Assert.AreEqual(_pixels.Length, length);

            NativeArray<float4> pixels = new NativeArray<float4>(length, Allocator.TempJob);

            for (int i = 0; i < _pixels.Length; i++)
            {
                pixels[i] = _pixels[i].ToFloat4();
            }

            NebulaJobs.CalcFinalColorPass jobCalcFinalColorPass = new NebulaJobs.CalcFinalColorPass
            {
                contrast = contrast,
                brightness = brightness,
                minAlpha = minAlpha,
                maxAlpha = maxAlpha,
                pixels = pixels,
            };
            JobHandle handleCalcFinalColorPass = jobCalcFinalColorPass.Schedule(length, 1);
            handleCalcFinalColorPass.Complete();

            for (int i = 0; i < length; i++)
            {
                this._pixels[i] = new Color(
                    pixels[i].x,
                    pixels[i].y,
                    pixels[i].z,
                    pixels[i].w
                );
            }

            pixels.Dispose();
        }

        void InitNoise(ref float[] noiseArray)
        {
            if (noiseArray == null || noiseArray.Length != width * height)
            {
                noiseArray = new float[width * height];
            }
            Assert.AreEqual(noiseArray.Length, width * height);
        }

        void InitPixels(ref Color[] pixelsArray)
        {
            if (pixelsArray == null || pixelsArray.Length != width * height)
            {
                pixelsArray = new Color[width * height];
            }
            Assert.AreEqual(pixelsArray.Length, width * height);
        }

        void InitColorLerps(ref float2[] colorLerps)
        {
            if (colorLerps == null || colorLerps.Length != width * height)
            {
                colorLerps = new float2[width * height];
            }
            Assert.AreEqual(_colorLerps.Length, width * height);
        }

        Color[] NoiseToColor(float[] noise)
        {
            Color[] colors = new Color[noise.Length];
            // NormalizeNoise(ref noise, 0f, 1f);
            for (int i = 0; i < noise.Length; i++)
            {
                colors[i] = new Color(
                    noise[i],
                    noise[i],
                    noise[i],
                    1f
                );
            }
            return colors;
        }

        Texture2D ColorToTexture2D(Color[] colors)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.SetPixels(colors);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        Color[] Texture2DToColor(Texture2D texture)
        {
            Assert.AreEqual(texture.width, width);
            Assert.AreEqual(texture.height, height);
            Color[] colors = texture.GetPixels();
            return colors;
        }

        Texture2D NoiseToTexture2D(float[] noise)
        {
            Color[] colors = NoiseToColor(noise);
            return ColorToTexture2D(colors);
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            shouldGenerate = true;
        }
    }
}
