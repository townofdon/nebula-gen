
using System.Collections.Generic;
using UnityEngine;

namespace NebulaGen
{

    public class Nebula : MonoBehaviour
    {
        [SerializeField] bool debug;
        [SerializeField] Color baseColor = Color.blue;
        [SerializeField] Color fillColor = Color.cyan;
        [SerializeField] Color highlightColor = Color.white;
        [SerializeField] Color backgroundColor = Color.black;
        [SerializeField][Range(0f, 1f)] float maxAlpha = 0.9f;

        [Space]
        [Space]

        [SerializeField][Range(1f, 100)] int baseThreshold = 1;
        [SerializeField][Range(1f, 100)] int fillThreshold = 5;
        [SerializeField][Range(1f, 100)] int highlightThreshold = 10;

        Color[] pixels;
        Color[] output;
        int[] distances;
        int maxDistanceFound;
        int width;
        int height;

        const int OUT_OF_BOUNDS = int.MaxValue;

        Cardinal[] cardinals = new Cardinal[] {
        Cardinal.N,
        Cardinal.S,
        Cardinal.W,
        Cardinal.E,
        Cardinal.NW,
        Cardinal.NE,
        Cardinal.SW,
        Cardinal.SE,
    };

        enum Cardinal
        {
            N,
            S,
            W,
            E,
            NW,
            NE,
            SW,
            SE,
        }

        public Color[] Generate(Texture2D source)
        {
            pixels = source.GetPixels();
            width = source.width;
            height = source.height;

            CalculateDistances();
            CalculateOutput();

            Debug.Log(maxDistanceFound);

            return output;
        }

        void CalculateDistances()
        {
            distances = new int[pixels.Length];
            int maxDistance = baseThreshold + fillThreshold + highlightThreshold;

            for (int i = 0; i < pixels.Length; i++)
            {
                Dictionary<Cardinal, int> pixelCheck = GetPixelCheck(i, maxDistance);
                distances[i] = GetClosestDistance(pixelCheck, maxDistance);
                if (distances[i] > maxDistanceFound) maxDistanceFound = distances[i];


                if (i % width == width / 2 && i / width == height / 2)
                {
                    Debug.Log($"N={pixelCheck[Cardinal.N]}");
                    Debug.Log($"S={pixelCheck[Cardinal.S]}");
                    Debug.Log($"W={pixelCheck[Cardinal.W]}");
                    Debug.Log($"E={pixelCheck[Cardinal.E]}");
                    Debug.Log($"NW={pixelCheck[Cardinal.NW]}");
                    Debug.Log($"NE={pixelCheck[Cardinal.NE]}");
                    Debug.Log($"SW={pixelCheck[Cardinal.SW]}");
                    Debug.Log($"SE={pixelCheck[Cardinal.SE]}");
                }
            }
        }

        void CalculateOutput()
        {
            output = new Color[distances.Length];
            int maxDistance = baseThreshold + fillThreshold + highlightThreshold;

            for (int i = 0; i < distances.Length; i++)
            {
                int distance = distances[i];
                if (debug)
                {
                    float val = (float)distance / (float)maxDistanceFound;
                    output[i] = new Color(val, val, val, 1f);
                    continue;
                }
                bool isHighlight = distance >= baseThreshold + fillThreshold + highlightThreshold;
                bool isFill = distance >= baseThreshold + fillThreshold;
                bool isBase = distance >= baseThreshold;
                if (isHighlight)
                {
                    output[i] = highlightColor;
                }
                else if (isFill)
                {
                    output[i] = fillColor;
                }
                else if (isBase)
                {
                    output[i] = baseColor;
                }
                else
                {
                    output[i] = backgroundColor;
                }
            }
        }

        int GetClosestDistance(Dictionary<Cardinal, int> pixelCheck, int maxDistance)
        {
            int closestDistance = maxDistance;
            foreach (var cardinal in cardinals)
            {
                if (pixelCheck[cardinal] < closestDistance) closestDistance = pixelCheck[cardinal];
                if (closestDistance == 0) return 0;
            }
            return closestDistance;
        }

        Dictionary<Cardinal, int> GetPixelCheck(int index, int maxDistance)
        {
            Dictionary<Cardinal, int> pixelCheck = new Dictionary<Cardinal, int>();
            foreach (var cardinal in cardinals)
            {
                int distance = GetPixelDistanceToEdge(index, cardinal, maxDistance);
                pixelCheck[cardinal] = distance;
            }
            return pixelCheck;
        }

        int GetPixelDistanceToEdge(int indexStart, Cardinal cardinal, int maxDistance)
        {
            int distance = 0;
            int index = indexStart;
            if (IsVoidPixel(index)) return 0;
            for (int i = 0; i <= maxDistance; i++)
            {
                distance++;
                switch (cardinal)
                {
                    case Cardinal.N:
                        index = GetPixelUp(index);
                        break;
                    case Cardinal.S:
                        index = GetPixelDown(index);
                        break;
                    case Cardinal.W:
                        index = GetPixelLeft(index);
                        break;
                    case Cardinal.E:
                        index = GetPixelRight(index);
                        break;
                    case Cardinal.NW:
                        index = GetPixelUp(GetPixelLeft(index));
                        break;
                    case Cardinal.NE:
                        index = GetPixelUp(GetPixelRight(index));
                        break;
                    case Cardinal.SW:
                        index = GetPixelDown(GetPixelLeft(index));
                        break;
                    case Cardinal.SE:
                        index = GetPixelDown(GetPixelRight(index));
                        break;
                }
                if (IsVoidPixel(index)) return distance;
            }
            return distance;
        }

        bool IsVoidPixel(int index)
        {
            if (index == OUT_OF_BOUNDS) return true;
            if (pixels[index].b <= Mathf.Epsilon) return true;
            if (pixels[index].a <= Mathf.Epsilon) return true;
            return false;
        }

        int GetPixelRight(int index)
        {
            if (index == OUT_OF_BOUNDS) return OUT_OF_BOUNDS;
            int newIndex = index + 1;
            if (newIndex >= pixels.Length) return OUT_OF_BOUNDS;
            // note - using integer division here
            if (newIndex / width != index / width) return OUT_OF_BOUNDS;
            return newIndex;
        }

        int GetPixelLeft(int index)
        {
            if (index == OUT_OF_BOUNDS) return OUT_OF_BOUNDS;
            int newIndex = index - 1;
            if (newIndex < 0) return OUT_OF_BOUNDS;
            return newIndex;
        }

        int GetPixelUp(int index)
        {
            if (index == OUT_OF_BOUNDS) return OUT_OF_BOUNDS;
            int newIndex = index + width;
            if (newIndex >= pixels.Length) return OUT_OF_BOUNDS;
            return newIndex;
        }

        int GetPixelDown(int index)
        {
            if (index == OUT_OF_BOUNDS) return OUT_OF_BOUNDS;
            int newIndex = index - width;
            if (newIndex < 0) return OUT_OF_BOUNDS;
            return newIndex;
        }
    }
}
