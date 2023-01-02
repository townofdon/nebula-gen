
using UnityEngine;

public static class PaletteUtils
{
    public static string[] getHexArrayFromColors(Color[] colors)
    {
        string[] hexArray = new string[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            hexArray[i] = ColorToHex(colors[i]);
        }
        return hexArray;
    }

    public static Color[] getColorsArrayFromHex(string[] hexArray)
    {
        Color[] colors = new Color[hexArray.Length];
        for (int i = 0; i < hexArray.Length; i++)
        {
            colors[i] = HexToColor(hexArray[i]);
        }
        return colors;
    }

    // Note that Color32 and Color implictly convert to each other.
    // You may pass a Color object to this method without first casting it.
    public static string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(string hex)
    {
        if (hex.Length < 6)
        {
            throw new UnityException("Hexadecimal Color Value is too short!");
        }
        else
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
