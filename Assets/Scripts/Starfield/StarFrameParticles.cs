using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StarFrameParticles : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField][Range(0.1f, 1f)] float parallaxDepth = 0.5f;
    [SerializeField][Range(0, 1000)] int numStars = 10;
    [SerializeField][Range(0, 1000)] int numStarsVariance = 2;

    [SerializeField] float starSize = 0.1f;
    [SerializeField][Range(0f, 1f)] float starSizeRange = 0.5f;
    [SerializeField] bool colorize = false;

    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] stars;

    ParticleSystemRenderer particleSystemRenderer;
    MaterialPropertyBlock particleRendererMaterialBlock;

    bool warpEnabled = false;

    enum FrameShiftDirection
    {
        NOCHANGE,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    public void SetWarpEnabled(bool value)
    {
        warpEnabled = value;
        // TODO: enable trail renderers on particle system
    }

    (int x, int y) index = (x: 0, y: 0);
    (int x, int y) prevIndex = (x: 0, y: 0);

    Vector2 lowerLeftBounds;
    Vector2 upperRightBounds;
    Vector2 cameraPosition;
    float screenWidth;
    float screenHeight;

    const float GOLDEN_RATIO = 1.61803398874989484820458683436563811772f;
    const float PI = Mathf.PI;

    Vector2 frameSeedStep;
    Vector2 starSeedStep;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        Assert.IsNotNull(particleSystem, "Missing ParticleSystem!");
        particleRendererMaterialBlock = new MaterialPropertyBlock();
        particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        float maxPossibleStars = Mathf.CeilToInt(numStars + numStarsVariance * 0.5f);
        frameSeedStep = new Vector2(GOLDEN_RATIO * 100f, PI * 100f);
        starSeedStep = frameSeedStep / maxPossibleStars;
        index.x = 0;
        index.y = 0;
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        if (debug) Debug.Log("Init");
        yield return new WaitForEndOfFrame();
        if (debug) Debug.Log("Populating stars...");
        PopulateStars();
        if (debug) Debug.Log("Populated stars.");
        if (debug) Debug.Log("\n\n");
    }

    void Update()
    {
        prevIndex = index;
        index = GetFrameIndexFromCameraPosition();
        CalculateStarPositions();
    }

    void LateUpdate()
    {
        transform.position = cameraPosition * parallaxDepth;
    }

    void OnDrawGizmos()
    {
        if (!debug) return;
        Gizmos.color = Color.yellow;
        Vector2 offset = (Vector2)transform.position + Vector2.right * screenWidth * index.x + Vector2.up * screenHeight * index.y;
        Vector2 TL = offset - Vector2.right * screenWidth * 0.5f + Vector2.up * screenHeight * 0.5f;
        Vector2 TR = offset + Vector2.right * screenWidth * 0.5f + Vector2.up * screenHeight * 0.5f;
        Vector2 BL = offset - Vector2.right * screenWidth * 0.5f - Vector2.up * screenHeight * 0.5f;
        Vector2 BR = offset + Vector2.right * screenWidth * 0.5f - Vector2.up * screenHeight * 0.5f;
        Gizmos.DrawLine(TL, TR);
        Gizmos.DrawLine(TR, BR);
        Gizmos.DrawLine(BR, BL);
        Gizmos.DrawLine(BL, TL);
    }

    FrameShiftDirection GetChanged()
    {
        // true if frame has moved one "unit"
        CalcScreen();
        return FrameShiftDirection.NOCHANGE;
    }

    void CalcScreen()
    {
        lowerLeftBounds = CameraUtils.GetMainCamera().ViewportToWorldPoint(Vector2.zero);
        upperRightBounds = CameraUtils.GetMainCamera().ViewportToWorldPoint(Vector2.one);
        screenWidth = upperRightBounds.x - lowerLeftBounds.x;
        screenHeight = upperRightBounds.y - lowerLeftBounds.y;
        cameraPosition = CameraUtils.GetMainCamera().transform.position;
    }

    string ToMapKey((int x, int y) frameIndex)
    {
        return $"{frameIndex.x}::{frameIndex.y}";
    }

    void PopulateStars()
    {
        CalcScreen();
        // TODO: ADD PERLIN NOISE INSTEAD OF RANDOM
        // see - https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
        int numToPopulate = Mathf.Max(0, numStars + Random.Range(-numStarsVariance, numStarsVariance));
        stars = new ParticleSystem.Particle[numToPopulate];

        for (int i = 0; i < numToPopulate; i++)
        {
            float randSize = Random.Range(1f - starSizeRange, starSizeRange + 1f);
            stars[i].startSize = starSize * randSize;
            stars[i].position = RandomVector2(lowerLeftBounds - cameraPosition, upperRightBounds - cameraPosition);
            float scaledColor = (true == colorize) ? randSize - starSizeRange : 1f;
            stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
            particleSystem.SetParticles(stars, stars.Length);
        }

        if (debug) Debug.Log($"added {numToPopulate} stars");
    }

    Vector3 _tempPosition;
    Vector2 _shift;

    void CalculateStarPositions()
    {
        if (stars == null) return;

        for (int i = 0; i < stars.Length; i++)
        {
            _tempPosition = stars[i].position + transform.position;
            // wrap star position by screen size
            _shift.x = Mathf.Sign(cameraPosition.x - _tempPosition.x) * Mathf.Floor(Mathf.Abs((cameraPosition.x - _tempPosition.x) / (screenWidth * 0.5f)));
            _shift.y = Mathf.Sign(cameraPosition.y - _tempPosition.y) * Mathf.Floor(Mathf.Abs((cameraPosition.y - _tempPosition.y) / (screenHeight * 0.5f)));
            _tempPosition.x += _shift.x * screenWidth;
            _tempPosition.y += _shift.y * screenHeight;
            stars[i].position = _tempPosition - transform.position;
        }

        particleSystem.SetParticles(stars, stars.Length);
    }

    Vector2 RandomVector2(Vector2 min, Vector2 max)
    {
        // Random.InitState(Mathf.RoundToInt(seed.x));
        float x = UnityEngine.Random.Range(min.x, max.x);
        // Random.InitState(Mathf.RoundToInt(seed.y));
        float y = UnityEngine.Random.Range(min.y, max.y);
        return new Vector2(x, y);
    }

    (int x, int y) GetFrameIndexFromCameraPosition()
    {
        CalcScreen();
        int x = Mathf.RoundToInt(cameraPosition.x / screenWidth * (1 - parallaxDepth));
        int y = Mathf.RoundToInt(cameraPosition.y / screenHeight * (1 - parallaxDepth));
        return (x, y);
    }
}
