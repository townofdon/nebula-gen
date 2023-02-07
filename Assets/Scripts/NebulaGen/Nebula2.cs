
using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using CyberneticStudios.SOFramework;

// TODO
// BACKBURNER
// - [ ] add save-file serialization && import feature
// - [ ] Add custom texture border falloffs (star pattern, diamond, etc.)
// - [ ] Change ColorPalette to ScriptableObject
// - [ ] Fix bug: turning mask on/off causes weirdness - seems to be related to non-standard canvas size
// - [ ] add drawable masking - show noise overlay with low alpha
// - [ ] add help section - instructions, keyboard shortcuts
// DONE
// - [x] Add contrast curve options
// - [x] Change to 100% opacity
// - [x] Add dithering option
// - [x] Add toast when saving image
// - [x] only generate image on G press (CTRL+S)
// - [x] Add more color palettes
// - [x] Split falloff variance into freq, strength - maybe also offset
// - [x] Remove old NoiseField classes
// - [x] Refactor NebulaGen fields to all use FloatVariables
// - [x] Add NoiseLayerB `solo` toggle option
// - [x] Enable camera controls while Adjust tab active
// - [x] add download button
// - [x] Add specific border falloff (top, down, left, right)
// - [x] Add exciting fancy noise textures
// - [x] Add mask fields
// - [x] Add focus outline for checkbox/toggle component
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
        [SerializeField] Toast toastSaveSuccess;

        [Space]
        [Space]

        [Header("Debouncing")]
        [SerializeField][Range(0f, 2f)] float repaintDelay = 0.2f;
        [SerializeField][Range(0f, 2f)] float redrawDelay = 0.2f;

        [Space]
        [Space]

        [Header("Custom Textures")]
        [SerializeField] CustomNoiseTexture customTextureNoiseA;
        [SerializeField] CustomNoiseTexture customTextureNoiseB;
        [SerializeField] CustomNoiseTexture customTextureMask;
        [SerializeField] CustomNoiseTexture customTextureBorder;

        [Space]
        [Space]

        [Header("Dimensions")]
        [SerializeField] public int sizeX = 480;
        [SerializeField] public int sizeY = 480;

        public const int noiseWidth = 480;
        public const int noiseHeight = 480;
        public const float noiseWidthQuotient = 1f / 480;
        public const float noiseHeightQuotient = 1f / 480;

        [Space]
        [Space]

        [Header("Noise")]
        [SerializeField]
        [FormerlySerializedAs("noiseLayerA")]
        NoiseOptions noiseOptionsA = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.3f,
            perlinOffset = Vector2.one * 10f,
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
            domainShiftAmount = 50f,
            swirlAmount = 0f,
            swirlIntensity = 1f,
            warpAmount = 0f,
            warpIntensity = 1f,
            mixAmount = 1f,
        };

        NoiseOptions noiseOptionsB = new NoiseOptions
        {
            noiseMode = FBMNoiseMode.Default,
            perlinFactor = 0.3f,
            perlinOffset = Vector2.one * 10f,
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
            domainShiftAmount = 50f,
            swirlAmount = 0f,
            swirlIntensity = 1f,
            warpAmount = 0f,
            warpIntensity = 1f,
            mixAmount = 1f,
        };
        // NOISE A
        [SerializeField] NoiseModeVariable noiseA_noiseMode;
        [SerializeField] NoiseTypeVariable noiseA_noiseType;
        [SerializeField] FloatVariable noiseA_perlinFactor;
        [SerializeField] FloatVariable noiseA_perlinOffsetX;
        [SerializeField] FloatVariable noiseA_perlinOffsetY;
        [SerializeField] FloatVariable noiseA_octaves;
        [SerializeField] FloatVariable noiseA_persistence;
        [SerializeField] FloatVariable noiseA_lacunarity;
        [SerializeField] FloatVariable noiseA_domainShiftPasses;
        [SerializeField] FloatVariable noiseA_domainShiftAmount;
        [SerializeField] FloatVariable noiseA_swirlAmount;
        [SerializeField] FloatVariable noiseA_swirlIntensity;
        [SerializeField] FloatVariable noiseA_warpAmount;
        [SerializeField] FloatVariable noiseA_warpIntensity;
        // NOISE B
        [SerializeField] BoolVariable isNoiseBEnabled;
        [SerializeField] BoolVariable isNoiseBSolo;
        [SerializeField] FloatVariable noiseBMix;
        [SerializeField] NoiseTypeVariable noiseB_noiseType;
        [SerializeField] FloatVariable noiseB_perlinFactor;
        [SerializeField] FloatVariable noiseB_perlinOffsetX;
        [SerializeField] FloatVariable noiseB_perlinOffsetY;
        [SerializeField] FloatVariable noiseB_octaves;
        [SerializeField] FloatVariable noiseB_persistence;
        [SerializeField] FloatVariable noiseB_lacunarity;
        [SerializeField] FloatVariable noiseB_domainShiftPasses;
        [SerializeField] FloatVariable noiseB_domainShiftAmount;
        [SerializeField] FloatVariable noiseB_swirlAmount;
        [SerializeField] FloatVariable noiseB_swirlIntensity;
        [SerializeField] FloatVariable noiseB_warpAmount;
        [SerializeField] FloatVariable noiseB_warpIntensity;
        // applies to composite noise only
        [SerializeField] FloatVariable noiseMinCutoff;
        [SerializeField] FloatVariable noiseMaxCutoff;

        [Header("Mask")]

        [SerializeField][Range(0, 1)] public float maskSelectPoint = 0.4f;
        [SerializeField][Range(0, 1)] public float maskFalloff = 0.1f;
        [SerializeField][Range(0, 1)] public float maskSoftness = 0.1f;
        [SerializeField]
        [FormerlySerializedAs("maskLayerA")]
        public NoiseOptions maskOptionsA = new NoiseOptions
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
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("maskLayerB")]
        public NoiseOptions maskOptionsB = new NoiseOptions
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
        [SerializeField] FloatVariable maskPerlinFactor;
        [SerializeField] FloatVariable maskPerlinOffsetX;
        [SerializeField] FloatVariable maskPerlinOffsetY;
        [SerializeField] FloatVariable maskOctaves;
        [SerializeField] FloatVariable maskPersistence;
        [SerializeField] FloatVariable maskLacunarity;
        [SerializeField] FloatVariable maskDomainShiftPasses;
        [SerializeField] FloatVariable maskDomainShiftAmount;

        [Space]
        [Space]

        [Header("Mix")]
        [SerializeField][Range(0f, 1f)] public float mixMask = 1f;
        [SerializeField] AnimationCurveVariable outputCurve;

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
        [SerializeField] FloatVariable noiseBlackPoint;

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
        [SerializeField] FloatVariable falloffAddLeft;
        [SerializeField] FloatVariable falloffAddRight;
        [SerializeField] FloatVariable falloffAddTop;
        [SerializeField] FloatVariable falloffAddBottom;

        [Space]
        [Space]

        [Header("Pixel Art")]

        [SerializeField] BoolVariable enableDithering;
        [SerializeField] FloatVariable ditherThreshold;

        int pixelWidth;
        int pixelHeight;

        Color[] _pixels;
        float[] _noise;
        float[] _maskComposite;
        float2[] _colorLerps;

        float[] _customSourceNoise;
        float[] _customSourceMask;
        float[] _customSourceBorder;

        bool shouldGenerate = true;
        float timeElapsedSinceGenerating = float.MaxValue;

        bool shouldDraw = true;
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
            toastSaveSuccess.Show();
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

        // public NoiseType CurrentNoiseType => noiseOptionsA.noiseType;

        public void SetBorderMode(BorderMode incoming)
        {
            borderMode = incoming;
            OnBorderModeChange?.Invoke(incoming);
        }

        public Action<bool> OnMaskEnabledChange;
        public bool IsMaskEnabled => mixMask > 0;

        public void SetMaskMix(float value)
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
            Init();
            TryGenerate();
            TryDrawOutput();
        }

        void Init()
        {
            pixelWidth = sizeX;
            pixelHeight = sizeY;
            InitNoise(ref _customSourceNoise);
            InitNoise(ref _customSourceMask);
            InitNoise(ref _customSourceBorder);
            InitNoise(ref _noise);
            InitNoise(ref _maskComposite);
            InitPixels(ref _pixels);
            InitColorLerps(ref _colorLerps);
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
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, pixelWidth, pixelHeight), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
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
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, pixelWidth, pixelHeight), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
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
            CalcPixels();
            Assert.AreEqual(_pixels.Length, pixelWidth * pixelHeight);
            if (outputSprite.sprite == null || outputSprite.sprite.texture.width != pixelWidth || outputSprite.sprite.texture.height != pixelHeight)
            {
                // gen new backgrouns
                Color[] blackPixels = new Color[pixelWidth * pixelHeight];
                for (int i = 0; i < blackPixels.Length; i++) blackPixels[i] = new Color(0, 0, 0, 1);
                Texture2D bgTexture = ColorToTexture2D(blackPixels);
                Sprite bg = Sprite.Create(bgTexture, new Rect(0, 0, pixelWidth, pixelHeight), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
                bgSprite.sprite = bg;
                // apply colors
                Texture2D texture = ColorToTexture2D(_pixels);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, pixelWidth, pixelHeight), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.Tight, Vector4.zero, false);
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
            JobProps props = new JobProps
            {
                width = noiseWidth,
                height = noiseHeight,
                tilingFill = borderMode == BorderMode.Tile ? tilingFill : 0,
            };
            int length = noiseWidth * noiseHeight;

            NativeArray<float> noiseA = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseB = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> noiseFalloff = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask1 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> mask2 = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float> falloff = new NativeArray<float>(length, Allocator.TempJob);
            NativeArray<float2> colorLerps = new NativeArray<float2>(length, Allocator.TempJob);

            #region NOISE_CALC
            noiseOptionsA.mixAmount = (isNoiseBEnabled.value && isNoiseBSolo.value) ? 0f : 1f;
            noiseOptionsB.mixAmount = (isNoiseBEnabled.value ? 1f : 0f) * noiseBMix.value;
            // noiseOptionsA.mixAmount = 1f;
            // noiseOptionsB.mixAmount = 0f;
            maskOptionsA.mixAmount = 1f;
            maskOptionsB.mixAmount = 1f;
            falloffOptions.mixAmount = 1f;
            // map noiseA options to FloatVariables
            noiseOptionsA.noiseMode = noiseA_noiseMode.value;
            noiseOptionsA.noiseType = noiseA_noiseType.value;
            noiseOptionsA.perlinFactor = noiseA_perlinFactor.value;
            noiseOptionsA.perlinOffset.x = noiseA_perlinOffsetX.value;
            noiseOptionsA.perlinOffset.y = noiseA_perlinOffsetY.value;
            noiseOptionsA.octaves = (int)noiseA_octaves.value;
            noiseOptionsA.persistence = noiseA_persistence.value;
            noiseOptionsA.lacunarity = noiseA_lacunarity.value;
            noiseOptionsA.domainShiftPasses = (int)noiseA_domainShiftPasses.value;
            noiseOptionsA.domainShiftAmount = noiseA_domainShiftAmount.value;
            noiseOptionsA.swirlAmount = noiseA_swirlAmount.value;
            noiseOptionsA.swirlIntensity = noiseA_swirlIntensity.value;
            noiseOptionsA.warpAmount = noiseA_warpAmount.value;
            noiseOptionsA.warpIntensity = noiseA_warpIntensity.value;
            // map noiseB options to FloatVariables
            noiseOptionsB.noiseType = noiseB_noiseType.value;
            noiseOptionsB.perlinFactor = noiseB_perlinFactor.value;
            noiseOptionsB.perlinOffset.x = noiseB_perlinOffsetX.value;
            noiseOptionsB.perlinOffset.y = noiseB_perlinOffsetY.value;
            noiseOptionsB.octaves = (int)noiseB_octaves.value;
            noiseOptionsB.persistence = noiseB_persistence.value;
            noiseOptionsB.lacunarity = noiseB_lacunarity.value;
            noiseOptionsB.domainShiftPasses = (int)noiseB_domainShiftPasses.value;
            noiseOptionsB.domainShiftAmount = noiseB_domainShiftAmount.value;
            noiseOptionsB.swirlAmount = noiseB_swirlAmount.value;
            noiseOptionsB.swirlIntensity = noiseB_swirlIntensity.value;
            noiseOptionsB.warpAmount = noiseB_warpAmount.value;
            noiseOptionsB.warpIntensity = noiseB_warpIntensity.value;
            // map mask options to FloatVariables
            maskOptionsB.perlinFactor = maskPerlinFactor.value;
            maskOptionsB.perlinOffset.x = maskPerlinOffsetX.value;
            maskOptionsB.perlinOffset.y = maskPerlinOffsetY.value;
            maskOptionsB.octaves = (int)maskOctaves.value;
            maskOptionsB.persistence = maskPersistence.value;
            maskOptionsB.lacunarity = maskLacunarity.value;
            maskOptionsB.domainShiftPasses = (int)maskDomainShiftPasses.value;
            maskOptionsB.domainShiftAmount = maskDomainShiftAmount.value;
            // set min, max values for noise
            noiseOptionsA.minCutoff = noiseMinCutoff.value;
            noiseOptionsA.maxCutoff = noiseMaxCutoff.value;

            if (noiseOptionsA.noiseType != NoiseType.CustomTexture)
            {
                NebulaJobs.CalcNoiseWithColorLerps jobNoise = new NebulaJobs.CalcNoiseWithColorLerps
                {
                    noise = noiseA,
                    colorLerps = colorLerps,
                    options = noiseOptionsA,
                    props = props,
                };
                JobHandle handleNoise = jobNoise.Schedule(length, 1);
                handleNoise.Complete();
            }
            if (noiseOptionsB.noiseType != NoiseType.CustomTexture)
            {
                NebulaJobs.CalcNoiseWithColorLerps jobNoise = new NebulaJobs.CalcNoiseWithColorLerps
                {
                    noise = noiseB,
                    colorLerps = colorLerps,
                    options = noiseOptionsB,
                    props = props,
                };
                JobHandle handleNoise = jobNoise.Schedule(length, 1);
                handleNoise.Complete();
            }

            NebulaJobs.CalcNoise jobNoiseFalloff = new NebulaJobs.CalcNoise
            {
                noise = noiseFalloff,
                options = falloffOptions,
                props = props,
            };
            NebulaJobs.CalcNoise jobMask1 = new NebulaJobs.CalcNoise
            {
                noise = mask1,
                options = maskOptionsA,
                props = props,
            };
            NebulaJobs.CalcNoise jobMask2 = new NebulaJobs.CalcNoise
            {
                noise = mask2,
                options = maskOptionsB,
                props = props,
            };

            JobHandle handleNoiseFalloff = jobNoiseFalloff.Schedule(length, 1);
            JobHandle handleMask1 = jobMask1.Schedule(length, 1);
            JobHandle handleMask2 = jobMask2.Schedule(length, 1);

            handleNoiseFalloff.Complete();
            handleMask1.Complete();
            handleMask2.Complete();
            #endregion NOISE_CALC

            #region CUSTOM_TEXTURE
            if (noiseOptionsA.noiseType == NoiseType.CustomTexture)
            {
                customTextureNoiseA.GetNoiseArray(ref noiseA, noiseOptionsA.mixAmount);
            }
            if (noiseOptionsB.noiseType == NoiseType.CustomTexture)
            {
                customTextureNoiseB.GetNoiseArray(ref noiseB, noiseOptionsB.mixAmount);
            }
            #endregion CUSTOM_TEXTURE

            #region NORM_PASS_ONE
            // this combines noise A+B and normalizes the result in one fell swoop
            NebulaJobs.NormalizeCombinedNoise jobNormNoise = new NebulaJobs.NormalizeCombinedNoise
            {
                noiseA = noiseA,
                noiseB = noiseB,
                options = noiseOptionsA,
            };
            NebulaJobs.NormalizeNoise jobNormNoiseFalloff = new NebulaJobs.NormalizeNoise
            {
                noise = noiseFalloff,
                options = falloffOptions,
            };
            JobHandle handleNormNoise = jobNormNoise.Schedule();
            JobHandle handleNormNoiseFalloff = jobNormNoiseFalloff.Schedule();
            handleNormNoise.Complete();
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
                falloffAddTop = (int)falloffAddTop.value,
                falloffAddBottom = (int)falloffAddBottom.value,
                falloffAddLeft = (int)falloffAddLeft.value,
                falloffAddRight = (int)falloffAddRight.value,
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
            JobHandle handleCompositeMask = jobCompositeMask.Schedule(length, 1);
            handleCompositeMask.Complete();
            #endregion COMPOSITING

            #region NORMALIZATION
            NebulaJobs.NormalizeNoise jobNormalizeMask = new NebulaJobs.NormalizeNoise
            {
                noise = mask,
                options = defaultNoiseOptions
            };
            NebulaJobs.NormalizeColorLerps jobNormalizeColorLerps = new NebulaJobs.NormalizeColorLerps
            {
                colorLerps = colorLerps,
            };
            JobHandle handleNormalizeMask = jobNormalizeMask.Schedule();
            JobHandle handleNormalizeColorLerps = jobNormalizeColorLerps.Schedule();
            handleNormalizeMask.Complete();
            handleNormalizeColorLerps.Complete();
            #endregion NORMALIZATION

            #region SUBTRACTION
            for (int i = 0; i < length; i++)
            {

                float mod = Mathf.Lerp(1f, mask[i], mixMask);
                float val = outputCurve.value.animationCurve.Evaluate(noiseA[i]) * Mathf.Clamp01(falloff[i]);
                val *= mod;
                noiseA[i] = val <= noiseBlackPoint.value ? 0f : val;
            }
            #endregion SUBTRACTION

            for (int i = 0; i < length; i++)
            {
                this._noise[i] = noiseA[i];
                this._maskComposite[i] = mask[i];
                this._colorLerps[i] = colorLerps[i];
            }

            noiseA.Dispose();
            noiseB.Dispose();
            mask.Dispose();
            noiseFalloff.Dispose();
            mask1.Dispose();
            mask2.Dispose();
            falloff.Dispose();
            colorLerps.Dispose();
        }

        void CalcPixels()
        {
            int length = pixelWidth * pixelHeight;
            int noiseLength = noiseWidth * noiseHeight;
            Assert.AreEqual(_pixels.Length, length);
            JobProps props = new JobProps
            {
                width = pixelWidth,
                height = pixelHeight,
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
                enableDithering = (enableDithering.value ? 1 : 0),
                ditherThreshold = ditherThreshold.value,
                blackPoint = noiseBlackPoint.value,
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
            int length = pixelWidth * pixelHeight;
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
            if (pixelsArray == null || pixelsArray.Length != pixelWidth * pixelHeight)
            {
                pixelsArray = new Color[pixelWidth * pixelHeight];
            }
            Assert.AreEqual(pixelsArray.Length, pixelWidth * pixelHeight);
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
            Texture2D texture = new Texture2D(pixelWidth, pixelHeight, TextureFormat.ARGB32, false);
            texture.SetPixels(colors);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        Color[] Texture2DToColor(Texture2D texture)
        {
            Assert.AreEqual(texture.width, pixelWidth);
            Assert.AreEqual(texture.height, pixelHeight);
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
