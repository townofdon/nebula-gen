using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum ColorCompareMode
{
    Hue,
    RGB,
    Weighted,
}

public static class PixelArtUtils
{

    // Given an array of colors with an arbitrary size, return N distinct colors
    public static Color[] ReduceColors(Color[] input, int numUniqueColors)
    {
        if (input.Length <= numUniqueColors) return input;

        Color[] colors = new Color[input.Length];
        List<Color[]> boxes = new List<Color[]>(0);
        List<Color[]> temp = new List<Color[]>(0);

        // init
        for (int i = 0; i < input.Length; i++) colors[i] = input[i];
        boxes.Add(colors);
        Assert.AreEqual(boxes.Count, 1);

        int passes = 0;
        while (boxes.Count < numUniqueColors)
        {
            foreach (var box in boxes)
            {
                Color[] newBox;
                Color max = Color.black;
                Color min = Color.white;
                foreach (var color in box)
                {
                    if (color.r > max.r) max.r = color.r;
                    if (color.g > max.g) max.g = color.g;
                    if (color.b > max.b) max.b = color.b;
                    if (color.a > max.a) max.a = color.a;
                    if (color.r < min.r) min.r = color.r;
                    if (color.g < min.g) min.g = color.g;
                    if (color.b < min.b) min.b = color.b;
                    if (color.a < min.a) min.a = color.a;
                }
                Color range = max - min;
                bool rHasLargestRange = range.r == Mathf.Max(range.r, range.g, range.b, range.a);
                bool gHasLargestRange = range.g == Mathf.Max(range.r, range.g, range.b, range.a);
                bool bHasLargestRange = range.b == Mathf.Max(range.r, range.g, range.b, range.a);
                if (rHasLargestRange)
                {
                    newBox = box.OrderBy(x => x.r).ToArray();
                }
                else if (gHasLargestRange)
                {
                    newBox = box.OrderBy(x => x.g).ToArray();
                }
                else if (bHasLargestRange)
                {
                    newBox = box.OrderBy(x => x.b).ToArray();
                }
                else
                {
                    newBox = box.OrderBy(x => x.a).ToArray();
                }
                // slice the array into two halves
                if (newBox.Length == 1)
                {
                    temp.Add(new Color[] { newBox[0] });
                }
                else if (newBox.Length == 2)
                {
                    temp.Add(new Color[] { newBox[0] });
                    temp.Add(new Color[] { newBox[1] });
                }
                else
                {
                    int median = Mathf.FloorToInt(newBox.Length * 0.5f);
                    temp.Add(newBox[0..median]);
                    temp.Add(newBox[median..^0]);
                }
            }

            boxes = new List<Color[]>(temp);
            temp.Clear();
            passes++;
            if (passes > 10) throw new UnityException("Infinite Loop!!!");
        }

        colors = new Color[boxes.Count];
        for (int i = 0; i < boxes.Count; i++)
        {
            colors[i] = GetAverageColor(boxes[i]);
        }

        Assert.IsTrue(boxes.Count == numUniqueColors || boxes.Count == Mathf.NextPowerOfTwo(numUniqueColors));
        Assert.IsTrue(colors.Length == numUniqueColors || colors.Length == Mathf.NextPowerOfTwo(numUniqueColors));

        return colors;
    }

    public static Color GetAverageColor(Color[] colors)
    {
        if (colors.Length == 0) return Color.black;
        Vector4 agg = Vector4.zero;
        foreach (var color in colors)
        {
            agg.x += color.r;
            agg.y += color.g;
            agg.z += color.b;
            agg.w += color.a;
        }
        return new Color(agg.x / colors.Length, agg.y / colors.Length, agg.z / colors.Length, agg.z / colors.Length);
    }

    // closed match for hues only:
    public static int GetClosestColor1(List<Color> colors, Color target)
    {
        var hue1 = target.GetHue();
        var diffs = colors.Select(n => GetHueDistance(n.GetHue(), hue1));
        var diffMin = diffs.Min(n => n);
        return diffs.ToList().FindIndex(n => n == diffMin);
    }

    // closed match in RGB space
    public static int GetClosestColor2(List<Color> colors, Color target)
    {
        var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n => n);
        return colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
    }

    // weighed distance using hue, saturation and brightness
    public static Color GetClosestColor3(Color[] colors, Color target, float saturationFactor, float brightnessFactor)
    {
        float hue1 = target.GetHue();
        var num1 = ColorNum(target, saturationFactor, brightnessFactor);

        float diff0 = 0f;
        float diff1 = 0f;
        for (int i = 0; i < colors.Length - 1; i++)
        {
            diff0 = Mathf.Abs(ColorNum(colors[i + 0], saturationFactor, brightnessFactor) - num1) + GetHueDistance(colors[i + 0].GetHue(), hue1);
            diff1 = Mathf.Abs(ColorNum(colors[i + 1], saturationFactor, brightnessFactor) - num1) + GetHueDistance(colors[i + 1].GetHue(), hue1);
            if (diff0 < diff1) return colors[i];
        }

        return Color.black;

        // var diffs = colors.Select(n =>
        //     Mathf.Abs(ColorNum(n, saturationFactor, brightnessFactor) - num1) +
        //     getHueDistance(n.GetHue(), hue1));
        // var diffMin = diffs.Min(x => x);
        // return diffs.ToList().FindIndex(n => n == diffMin);
    }

    public static Color GetMatchingColor(Color[] colors, Color target)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (target == colors[i]) return colors[i];
        }
        return Color.black;
    }

    // color brightness as perceived:
    static float GetBrightness(Color c)
    { return (c.r * 0.299f + c.g * 0.587f + c.b * 0.114f) / 256f; }

    // distance between two hues:
    static float GetHueDistance(float hue1, float hue2)
    {
        float d = Mathf.Abs(hue1 - hue2); return d > 180 ? 360 - d : d;
    }

    //  weighed only by saturation and brightness (from my trackbars)
    static float ColorNum(Color c, float saturationFactor, float brightnessFactor)
    {
        return c.GetSaturation() * saturationFactor + GetBrightness(c) * brightnessFactor;
    }

    // distance in RGB space
    static int ColorDiff(Color c1, Color c2)
    {
        return (int)Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r)
                             + (c1.g - c2.g) * (c1.g - c2.g)
                             + (c1.b - c2.b) * (c1.b - c2.b));
    }
}
