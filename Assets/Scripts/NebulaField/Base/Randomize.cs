using UnityEngine;
using Unity.Mathematics;
using NebulaGen;
using CyberneticStudios.SOFramework;

public class Randomize : MonoBehaviour
{
    [SerializeField] NoiseModeVariable noiseMode;
    [SerializeField] NoiseTypeVariable noiseType;
    [SerializeField] FloatVariable perlinFactor;
    [SerializeField] FloatVariable perlinOffsetX;
    [SerializeField] FloatVariable perlinOffsetY;
    [SerializeField] FloatVariable octaves;
    [SerializeField] FloatVariable persistence;
    [SerializeField] FloatVariable lacunarity;
    [SerializeField] FloatVariable domainShiftPasses;
    [SerializeField] FloatVariable domainShiftAmount;

    Nebula2 nebula2;

    public void RandomizeNoise()
    {
        if (noiseMode != null) noiseMode.value = GetRandomItem(new FBMNoiseMode[] {
            FBMNoiseMode.Default,
            FBMNoiseMode.Inverted,
            FBMNoiseMode.Ridges,
            FBMNoiseMode.Turbulence,
        });
        noiseType.value = GetRandomItem(new NoiseType[] {
            NoiseType.Perlin1,
            NoiseType.Perlin2,
            NoiseType.Worley1,
            NoiseType.Worley2,
            NoiseType.Simplex,
        });
        perlinFactor.value = RandomFloat(0.06f, 4f);
        perlinOffsetX.value = RandomFloat(0, 20);
        perlinOffsetY.value = RandomFloat(0, 20);
        octaves.value = RandomInt(1, 8);
        lacunarity.value = RandomFloat(1.5f, 2.5f);
        persistence.value = RandomFloat(0.25f, 0.75f);
        domainShiftPasses.value = RandomInt(0, 2);
        domainShiftAmount.value = RandomFloat(10f, 200f);
        AfterRandomize();
    }

    public void RandomizeMask()
    {
        AfterRandomize();
    }

    public void RandomizeBorder()
    {
        AfterRandomize();
    }

    void AfterRandomize()
    {
        nebula2.GenerateNoise();
        FieldEvent.OnReinitializeFields?.Invoke();
    }

    private void Awake()
    {
        nebula2 = FindObjectOfType<Nebula2>();
    }

    static T GetRandomItem<T>(T[] items)
    {
        int index = UnityEngine.Random.Range(0, items.Length);
        return items[index];
    }

    static float RandomFloat(float minInclusive, float maxInclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    static int RandomInt(int minInclusive, int maxExclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxExclusive);
    }
}