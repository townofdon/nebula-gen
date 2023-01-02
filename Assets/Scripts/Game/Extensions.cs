using Unity.Mathematics;
using UnityEngine;

public static class Extensions
{
    public static Vector2 heading(this Rigidbody2D rb)
    {
        return rb.velocity.normalized;
    }

    public static Color toAlpha(this Color color, float alpha)
    {
        Color temp = color;
        temp.a = Mathf.Clamp01(alpha);
        return temp;
    }

    public static void SetActiveAndEnable(this MonoBehaviour component, bool isActive)
    {
        component.enabled = isActive;
        component.gameObject.SetActive(isActive);
    }

    public static void SetActiveAndEnable(this Renderer renderer, bool isActive)
    {
        renderer.enabled = isActive;
        renderer.gameObject.SetActive(isActive);
    }

    public static float GetHue(this Color color)
    {
        float hue, saturation, value;
        Color.RGBToHSV(color, out hue, out saturation, out value);
        return hue;
    }

    public static float GetSaturation(this Color color)
    {
        float hue, saturation, value;
        Color.RGBToHSV(color, out hue, out saturation, out value);
        return saturation;
    }

    public static float GetValue(this Color color)
    {
        float hue, saturation, value;
        Color.RGBToHSV(color, out hue, out saturation, out value);
        return value;
    }

    public static float4 ToFloat4(this Color color)
    {
        return new float4(
            color.r,
            color.g,
            color.b,
            color.a
        );
    }
}
