using UnityEngine;
using Unity.Mathematics;
using NebulaGen;

public class Randomize : MonoBehaviour
{
    Nebula2 nebula2;

    public void RandomizeNoise()
    {
        nebula2.noiseOptions.noiseMode = GetRandomItem(new FBMNoiseMode[] {
            FBMNoiseMode.Default,
            FBMNoiseMode.Inverted,
            FBMNoiseMode.Ridges,
            FBMNoiseMode.Turbulence,
        });
        nebula2.noiseOptions.noiseType = GetRandomItem(new NoiseType[] {
            NoiseType.Perlin1,
            NoiseType.Perlin2,
            NoiseType.Worley1,
            NoiseType.Worley2,
            NoiseType.Simplex,
        });
        nebula2.noiseOptions.perlinFactor = RandomFloat(0.06f, 4f);
        nebula2.noiseOptions.perlinOffset = new float2(RandomFloat(0, 20), RandomFloat(0, 20));
        nebula2.noiseOptions.octaves = RandomInt(1, 8);
        nebula2.noiseOptions.lacunarity = RandomFloat(1.5f, 2.5f);
        nebula2.noiseOptions.persistence = RandomFloat(0.25f, 0.75f);
        nebula2.noiseOptions.domainShiftPasses = RandomInt(0, 2);
        nebula2.noiseOptions.domainShiftAmount = RandomFloat(10f, 200f);
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