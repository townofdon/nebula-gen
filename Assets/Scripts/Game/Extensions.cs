using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    const int MAX_UI_PARENT_DEPTH = 10;
    const string CONTENT_TAG = "ScrollViewContent";

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

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector3 viewportLocalPosition = instance.viewport.localPosition;
        Vector3 childLocalPosition = child.localPosition;
        Transform parent = child.transform.parent;
        int i = 0;
        while (parent != null && !parent.CompareTag(CONTENT_TAG) && parent != instance.transform && i < MAX_UI_PARENT_DEPTH)
        {
            childLocalPosition += parent.localPosition;
            parent = parent.parent;
            i++;
        }
        if (i >= MAX_UI_PARENT_DEPTH) Debug.LogWarning($"Prevented an infinite loop in GetSnapToPositionToBringChildIntoView for RectTransform {child.gameObject.name}");
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
