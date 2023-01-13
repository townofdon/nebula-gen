
using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

// MASK FIELDS
// - [ ] RANDOMIZE
// - [x] Mask select point
// - [x] Mask falloff
// - [ ] Mask Layer A/B: Noise Type
// - [ ] Mask Layer A/B: Noise Mode
// - [ ] Mask Layer A/B: perlinFactor (frequency)
// - [ ] Mask Layer A/B: perlinOffset (offset)
// - [ ] Mask Layer A/B: octaves
// - [ ] Mask Layer A/B: lacunarity
// - [ ] Mask Layer A/B: persistence
// - [ ] Mask Layer A/B: domainShiftPasses
// - [ ] Mask Layer A/B: domainShiftAmount

// TODO
// - [ ] Add mask fields
// - [ ] Fix bug: turning mask on/off causes weirdness - seems to be related to non-standard canvas size
// - [ ] Add exciting fancy noise textures
// - [ ] Change ColorPalette to ScriptableObject
// - [ ] Add more color palettes
// - [ ] add file section?? -> save icon in bottom-right corner, with tooltip
// - [ ] add help section - instructions, keyboard shortcuts
// - [ ] add download button
// BACKBURNER
// - [ ] only generate image on G press
// - [ ] add drawable masking - show noise overlay with low alpha
// DONE
// - [x] show mask preview in red
// - [x] Add octaves to falloff variance calc
// - [x] Make current noise value factor into strength of falloff variance
// - [x] Remove Draw view; add Border view
// - [x] add initial UI sections - see below
// - [x] get old project working
// - [x] get min/max noise fnc working (add separate normalize passes)
// - [x] add better tiling
// - [x] resample noise grid to pixels grid
// - [x] show distinct noise, output textures
// - [x] hook up drawable functionality
// - [x] add image download
// - [x] add drawable color highlighting, show brushhead
// - [x] add camera controls (pan, zoom)

// UI TABS
// General Settings
// Noise 1
// Noise 2 ($)
// Mask Drawing ($)
// Keyboard Shortcuts

// GENERAL SETTINGS
// - Canvas size {float, float}
// - Show stars {bool}
//   - Star density {float}
// - Enable Tiling {bool}
//   - Tiling Distance ($)
// - Show background {bool}
// - Button: Save to File

namespace NebulaGen
{

    public enum ColorMode
    {
        PixelArt,
        RGB,
    }

    public enum BorderMode
    {
        None,
        FalloffBox,
        FalloffCircle,
        Tile,
    }

    public class Nebula2 : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Header("Refs")]
        [SerializeField] SpriteRenderer bgSprite;
        [SerializeField] SpriteRenderer noiseSprite;
        [SerializeField] SpriteRenderer maskSprite;
        [SerializeField] SpriteRenderer outputSprite;
        [SerializeField][Range(0f, 2f)] float repaintDelay = 0.2f;
        [SerializeField][Range(0f, 2f)] float redrawDelay = 0.2f;

        [Space]
        [Space]

        [Header("Dimensions")]
        [SerializeField] public int sizeX = 480;
        [SerializeField] public int sizeY = 480;

        const int noiseWidth = 480;
        const int noiseHeight = 480;

        [Space]
        [Space]

        [Header("Noise")]
        [SerializeField]
        public NoiseOptions noiseLayerA = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.3f,
            perlinOffset = Vector2.one * 15f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            minCutoff = 0f,
            maxCutoff = 1f,
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
            minCutoff = 0f,
            maxCutoff = 1f,
            perlinFactor = 0.4f,
            perlinOffset = Vector2.one * 25f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
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

        [SerializeField][Range(0, 1)] public float maskSelectPoint = 0.4f;
        [SerializeField][Range(0, 1)] public float maskFalloff = 0.1f;
        [SerializeField][Range(0, 1)] public float maskSoftness = 0.1f;
        [SerializeField]
        public NoiseOptions maskLayerA = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            minCutoff = 0.2f,
            maxCutoff = 0.8f,
            perlinFactor = .3f,
            perlinOffset = Vector2.one * 5f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
            initialFreq = 1f,
            initialAmp = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
        };
        [SerializeField]
        public NoiseOptions maskLayerB = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            minCutoff = 0.2f,
            maxCutoff = 0.8f,
            perlinFactor = .35f,
            perlinOffset = Vector2.one * 10f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 1f,
            octaves = 8,
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
        [SerializeField][Range(0f, 1f)] public float mixMask = 1f;
        [SerializeField] AnimationCurve outputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

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
        [SerializeField] public BorderMode borderMode = BorderMode.FalloffBox;
        [SerializeField][Range(0f, 50f)] public float edgeDistance = 5f;
        [SerializeField][Range(0f, 1000f)] public float edgeFalloff = 5f;
        [SerializeField][Range(0f, 1000f)] public float edgeCutStrength = 5f;
        [SerializeField][Range(0f, 1000f)] public float edgeVarianceEffect = 20f;
        [SerializeField][Range(0f, 150f)] public float edgeVarianceStrength = 5f;
        [SerializeField][Range(0f, 100)] public int tilingFill = 50;
        [SerializeField]
        public NoiseOptions falloffOptions = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            minCutoff = 0.2f,
            maxCutoff = 0.8f,
            perlinFactor = .35f,
            perlinOffset = Vector2.one * 100f,
            voronoiAngleOffset = 0f,
            voronoiCellDensity = 5f,
            octaves = 4,
            initialFreq = 1f,
            initialAmp = 1f,
            persistence = 0.5f,
            lacunarity = 2.0f,
        };

        [Space]
        [Space]

        [Header("Pixel Art")]

        [SerializeField] bool enableDithering = true;
        [SerializeField][Range(0f, 1f)] float ditherThreshold = 0.25f;

        int width;
        int height;

        Color[] _pixels;
        float[] _noise;
        float[] _maskComposite;
        float2[] _colorLerps;

        bool shouldGenerate;
        float timeElapsedSinceGenerating = float.MaxValue;

        bool shouldDraw;
        float timeElapsedSinceDrawing = float.MaxValue;

        NoiseOptions defaultNoiseOptions = new NoiseOptions
        {
            normType = NormalizationType.Truncate,
            noiseMode = FBMNoiseMode.Default,
            minCutoff = 0,
            maxCutoff = 1,
        };

        public Action<ColorPalette.Palette> OnPaletteChange;
        public ColorPalette.Palette MainPalette => paletteMain;

        Coroutine clearingPrint;
        public void SaveImage()
        {
            Debug.Log("Saving...");
            ImageUtils.SaveImage(outputSprite.sprite.texture as Texture2D);
            Debug.Log("Save Successful!");
            if (clearingPrint != null) StopCoroutine(clearingPrint);
            clearingPrint = StartCoroutine(CClearPrint());
        }

        public void SetPalette(ColorPalette.Palette incoming)
        {
            paletteMain = incoming;
            if (OnPaletteChange != null) OnPaletteChange.Invoke(incoming);
        }

        public Action<BorderMode> OnBorderModeChange;
        public BorderMode CurrentBorderMode => borderMode;

        public void SetBorderMode(BorderMode incoming)
        {
            borderMode = incoming;
            OnBorderModeChange?.Invoke(incoming);
        }

        public Action<bool> OnMaskEnabledChange;
        public bool IsMaskEnabled => mixMask > 0;

        public void SetMaskEnabled(float value)
        {
            mixMask = value;
            OnMaskEnabledChange?.Invoke(value > 0);
        }

        public void GenerateNoise()
        {
            shouldGenerate = true;
        }

        public void DrawOutput()
        {
            shouldDraw = true;
        }

        void Start()
        {
            shouldGenerate = true;
            shouldDraw = true;
        }

        void Update()
        {
            TryGenerate();
            TryDrawOutput();
        }

        void TryGenerate()
        {
            if (!shouldGenerate) return;
            if (timeElapsedSinceGenerating < repaintDelay) return;
            shouldGenerate = false;
            GenerateNoiseImpl();
            timeElapsedSinceGenerating = 0f;
        }

        void TryDrawOutput()
        {
            if (!shouldDraw) return;
            if (timeElapsedSinceDrawing < redrawDelay) return;
            shouldDraw = false;
            DrawOutputImpl();
            timeElapsedSinceDrawing = 0f;
        }

        void LateUpdate()
        {
            timeElapsedSinceGenerating += Time.deltaTime;
            timeElapsedSinceDrawing += Time.deltaTime;
        }

        void GenerateNoiseImpl()
        {
            CalcNoise();
            if (noiseSprite.sprite == null || noiseSprite.sprite.texture.width != noiseWidth || noiseSprite.sprite.texture.height != noiseHeight)
            {
                Texture2D texture = NoiseToTexture2D(_noise, Color.white);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
                noiseSprite.sprite = sprite;
            }
            else
            {
                (noiseSprite.sprite.texture as Texture2D).SetPixels(NoiseToColor(_noise, Color.white));
                (noiseSprite.sprite.texture as Texture2D).Apply();
            }

            if (maskSprite.sprite == null || maskSprite.sprite.texture.width != noiseWidth || maskSprite.sprite.texture.height != noiseHeight)
            {
                Texture2D texture = NoiseToTexture2D(_maskComposite, Color.red, invert: true);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
                maskSprite.sprite = sprite;
            }
            else
            {
                (maskSprite.sprite.texture as Texture2D).SetPixels(NoiseToColor(_maskComposite, Color.red, invert: true));
                (maskSprite.sprite.texture as Texture2D).Apply();
            }
        }

        void DrawOutputImpl()
        {
            width = sizeX;
            height = sizeY;
            CalcPixels();
            Assert.AreEqual(_pixels.Length, width * height);
            if (outputSprite.sprite == null || outputSprite.sprite.texture.width != width || outputSprite.sprite.texture.height != height)
            {
                // gen new backgrouns
                Color[] blackPixels = new Color[width * height];
                for (int i = 0; i < blackPixels.Length; i++) blackPixels[i] = new Color(0, 0, 0, 1);
                Texture2D bgTexture = ColorToTexture2D(blackPixels);
                Sprite bg = Sprite.Create(bgTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
                bgSprite.sprite = bg;
                // apply colors
                Texture2D texture = ColorToTexture2D(_pixels);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
                outputSprite.sprite = sprite;
            }
            else
            {
                (outputSprite.sprite.texture as Texture2D).SetPixels(_pixels);
                (outputSprite.sprite.texture as Texture2D).Apply();
            }
        }

        void CalcNoise()
        {
            InitNoise(ref _noise);
            InitNoise(ref _maskComposite);
            InitColorLerps(ref _colorLerps);
            InitPixels(ref _pixels);

            JobProps props = new JobProps
            {
                width = noiseWidth,
                height = noiseHeight,
                tilingFill = borderMode == BorderMode.Tile ? tilingFill : 0,
            };
            int length = noiseWidth * noiseHeight;

            NativeArray<float> noise = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask = new NativeArray<float>(length, Allocator.TempJob);

            NativeArray<float> noiseA = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseB = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseFalloff = new NativeArray<float>(length, Allocator.TempJob);
            // NativeArray<float> noiseC = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask1 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask2 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> falloff = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float2> colorLerps = new NativeArray<float2>(length, Allocator.TempJob);

            #region NOISE_CALC
            noiseLayerA.mixAmount = mixNoiseA;
            noiseLayerB.mixAmount = mixNoiseB;
            // noiseLayerC.mixAmount = mixNoiseC;
            maskLayerA.mixAmount = 1f;
            maskLayerB.mixAmount = 1f;
            falloffOptions.mixAmount = 1f;
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
            NebulaJobs.CalcNoise jobNoiseFalloff = new NebulaJobs.CalcNoise
            {
                noise = noiseFalloff,
                options = falloffOptions,
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

            JobHandle handleNoiseA = jobNoiseA.Schedule(length, 1);
            JobHandle handleNoiseB = jobNoiseB.Schedule(length, 1);
            JobHandle handleNoiseFalloff = jobNoiseFalloff.Schedule(length, 1);
            JobHandle handleMask1 = jobMask1.Schedule(length, 1);
            JobHandle handleMask2 = jobMask2.Schedule(length, 1);

            handleNoiseA.Complete();
            handleNoiseB.Complete();
            handleNoiseFalloff.Complete();
            // handleNoiseC.Complete();
            handleMask1.Complete();
            handleMask2.Complete();
            #endregion NOISE_CALC

            #region NORM_PASS_ONE
            NebulaJobs.NormalizeNoise jobNormNoiseA = new NebulaJobs.NormalizeNoise
            {
                noise = noiseA,
                options = noiseLayerA,
            };
            NebulaJobs.NormalizeNoise jobNormNoiseB = new NebulaJobs.NormalizeNoise
            {
                noise = noiseB,
                options = noiseLayerB,
            };
            NebulaJobs.NormalizeNoise jobNormNoiseFalloff = new NebulaJobs.NormalizeNoise
            {
                noise = noiseFalloff,
                options = falloffOptions,
            };
            JobHandle handleNormNoiseA = jobNormNoiseA.Schedule();
            JobHandle handleNormNoiseB = jobNormNoiseB.Schedule();
            JobHandle handleNormNoiseFalloff = jobNormNoiseFalloff.Schedule();
            handleNormNoiseA.Complete();
            handleNormNoiseB.Complete();
            handleNormNoiseFalloff.Complete();
            #endregion NORM_PASS_ONE

            #region FALLOFF_CALC
            NebulaJobs.CalcFalloffJob jobFalloff = new NebulaJobs.CalcFalloffJob
            {
                falloff = falloff,
                props = props,
                noise = noiseA,
                noiseFalloff = noiseFalloff,
                amountBox = borderMode == BorderMode.FalloffBox ? 1f : 0f,
                amountCircle = borderMode == BorderMode.FalloffCircle ? 1f : 0f,
                edgeDistance = edgeDistance,
                edgeFalloff = edgeFalloff,
                edgeCutStrength = edgeCutStrength,
                edgeVarianceEffect = edgeVarianceEffect,
                edgeVarianceStrength = edgeVarianceStrength,
                // mixMask = mixMask,
                // maskSelectPoint = maskSelectPoint,
                // maskFalloff = maskFalloff,
            };
            JobHandle handleFalloff = jobFalloff.Schedule(length, 1);
            handleFalloff.Complete();
            #endregion

            #region COMPOSITING
            NebulaJobs.CalcCompositeMask jobCompositeMask = new NebulaJobs.CalcCompositeMask
            {
                mask1 = mask1,
                mask2 = mask2,
                falloff = falloff,
                maskSelectPoint = maskSelectPoint,
                maskFalloff = maskFalloff,
                maskSoftness = maskSoftness,
                mask = mask,
            };
            NebulaJobs.CalcCompositeNoise jobCompositeNoise = new NebulaJobs.CalcCompositeNoise
            {
                noiseA = noiseA,
                noiseB = noiseB,
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
                options = defaultNoiseOptions
            };
            NebulaJobs.NormalizeNoise jobNormalizeMask = new NebulaJobs.NormalizeNoise
            {
                noise = mask,
                options = defaultNoiseOptions
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
                float val = outputCurve.Evaluate(noise[i]) * Mathf.Clamp01(falloff[i]);
                // MULTIPLY
                val *= mod;
                // SUBTRACT
                // val = Mathf.Clamp01(val - (1 - mod));
                noise[i] = val <= blackPoint ? 0f : val;
            }
            #endregion SUBTRACTION

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
            noiseFalloff.Dispose();
            mask1.Dispose();
            mask2.Dispose();
            falloff.Dispose();
            colorLerps.Dispose();
        }

        void CalcPixels()
        {
            InitNoise(ref _noise);
            InitNoise(ref _maskComposite);
            InitColorLerps(ref _colorLerps);
            InitPixels(ref _pixels);

            int length = width * height;
            int noiseLength = noiseWidth * noiseHeight;
            Assert.AreEqual(_pixels.Length, length);
            JobProps props = new JobProps
            {
                width = width,
                height = height,
            };

            NativeArray<float> noise = new NativeArray<float>(noiseLength, Allocator.TempJob);
            NativeArray<float2> colorLerps = new NativeArray<float2>(noiseLength, Allocator.TempJob);
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
                noise[i] = _noise[i];
                colorLerps[i] = _colorLerps[i];
            }

            NebulaJobs.CalcColors jobCalcColorsPixelArt = new NebulaJobs.CalcColors
            {
                colorMode = colorMode,
                props = props,
                noiseWidth = noiseWidth,
                noiseHeight = noiseHeight,
                noise = noise,
                colorLerps = colorLerps,
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

            for (int i = 0; i < _pixels.Length; i++)
            {
                this._pixels[i] = new Color(
                    pixels[i].x,
                    pixels[i].y,
                    pixels[i].z,
                    pixels[i].w
                );
            }

            noise.Dispose();
            colorLerps.Dispose();
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
            if (noiseArray == null || noiseArray.Length != noiseWidth * noiseHeight)
            {
                noiseArray = new float[noiseWidth * noiseHeight];
            }
            Assert.AreEqual(noiseArray.Length, noiseWidth * noiseHeight);
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
            if (colorLerps == null || colorLerps.Length != noiseWidth * noiseHeight)
            {
                colorLerps = new float2[noiseWidth * noiseHeight];
            }
            Assert.AreEqual(_colorLerps.Length, noiseWidth * noiseHeight);
        }

        Color[] NoiseToColor(float[] noise, Color color, bool invert = false)
        {
            Color[] colors = new Color[noise.Length];
            for (int i = 0; i < noise.Length; i++)
            {
                colors[i] = new Color(
                    (invert ? 1 - noise[i] : noise[i]) * color.r,
                    (invert ? 1 - noise[i] : noise[i]) * color.g,
                    (invert ? 1 - noise[i] : noise[i]) * color.b,
                    (invert ? 1 - noise[i] : 1) * color.a
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

        Texture2D NoiseToTexture2D(float[] noise, Color color, bool invert = false)
        {
            Color[] colors = NoiseToColor(noise, color, invert);
            return ColorToTexture2D(colors);
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            shouldGenerate = true;
            shouldDraw = true;
        }

        IEnumerator CClearPrint()
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("");
        }
    }
}
