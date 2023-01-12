using Unity.Mathematics;


// see: https://easings.net/
public static class Easing
{
    public static float Linear(float x)
    {
        return x;
    }

    public static float InQuad(float x)
    {
        return x * x;
    }
    public static float InQuadInverse(float x)
    {
        return math.sqrt(x);
    }
    public static float OutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }
    public static float InOutQuad(float x)
    {
        return x < 0.5f ? 2f * x * x : 1f - math.pow(-2f * x + 2f, 2f) / 2f;
    }

    public static float InCubic(float x)
    {
        return x * x * x;
    }
    public static float OutCubic(float x)
    {
        return 1f - math.pow(1f - x, 3f);
    }
    public static float InOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - math.pow(-2f * x + 2f, 3f) / 2f;
    }

    public static float InQuart(float x)
    {
        return x * x * x * x;
    }
    public static float OutQuart(float x)
    {
        return 1f - math.pow(1 - x, 4f);
    }
    public static float InOutQuart(float x)
    {
        return x < 0.5f ? 8f * x * x * x * x : 1f - math.pow(-2f * x + 2f, 4f) / 2f;
    }

    public static float InQuint(float x)
    {
        return x * x * x * x * x;
    }
    public static float OutQuint(float x)
    {
        return 1f - math.pow(1f - x, 5f);
    }
    public static float InOutQuint(float x)
    {
        return x < 0.5f ? 16f * x * x * x * x * x : 1f - math.pow(-2f * x + 2f, 5f) / 2f;
    }

    public static float InExpo(float x)
    {
        return x == 0f ? 0f : math.pow(2f, 10f * x - 10f);
    }
    public static float OutExpo(float x)
    {
        return x == 1f ? 1f : 1f - math.pow(2f, -10f * x);
    }
    public static float InOutExpo(float x)
    {
        return x == 0f
            ? 0f
            : x == 1f
            ? 1f
            : x < 0.5f
            ? math.pow(2f, 20f * x - 10f) / 2f
            : (2f - math.pow(2f, -20f * x + 10f)) / 2f;
    }

    public static float InBack(float x, float backAmount = 1.70158f)
    {
        return (backAmount + 1f) * x * x * x - backAmount * x * x;
    }
    public static float OutBack(float x, float backAmount = 1.70158f)
    {
        return 1f + (backAmount + 1f) * math.pow(x - 1f, 3f) + backAmount * math.pow(x - 1f, 2f);
    }
    public static float InOutBack(float x, float backAmount = 1.70158f, float stabilize = 1.525f)
    {
        return x < 0.5f
            ? (math.pow(2f * x, 2f) * (((backAmount * stabilize) + 1f) * 2f * x - (backAmount * stabilize))) / 2f
            : (math.pow(2f * x - 2f, 2f) * (((backAmount * stabilize) + 1f) * (x * 2f - 2f) + (backAmount * stabilize)) + 2f) / 2f;
    }

    public static float OutCirc(float x)
    {
        return math.sqrt(1 - math.pow(x - 1, 2));
    }
}
