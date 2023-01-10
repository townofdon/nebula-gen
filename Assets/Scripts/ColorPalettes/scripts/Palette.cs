using UnityEngine;
using UnityEngine.Assertions;

// PROBLEMS:
// - I want to serialize palette data the "Unity way"
// - I want to easily access palette data when choosing colors from the Unity color picker
//   - This seems difficult.

namespace ColorPalette
{

    // [CreateAssetMenu(fileName = "Palette", menuName = "Palettes/Create Color Palette", order = 0)]
    public class Palette : MonoBehaviour
    {

        [SerializeField] PaletteData myData = null;

        public void Init()
        {
            if (myData == null)
            {
                myData = new PaletteData();
                myData.ResetToDefault();
            }
        }

        public string GetName()
        {
            return GetPaletteData().name;
        }

        public PaletteData GetPaletteData()
        {
            if (myData == null)
            {
                // initialize incase it's not done yet... should only be if it's used via editor script
                this.Init();
            }
            return myData;
        }

        public void SetPaletteData(PaletteData value)
        {
            myData = value;
        }

        public int GetNumColors()
        {
            if (myData == null) this.Init();
            return myData.colors.Length;
        }

        public Color GetColorByIndex(int index)
        {
            if (myData == null) this.Init();
            if (index >= myData.colors.Length) Debug.LogWarning($"index {index} out of bounds for Palette.myData.colors");
            return myData.colors[index % myData.colors.Length];
        }

        public float GetColorPercentageByIndex(int index)
        {
            if (myData == null) this.Init();
            if (index >= myData.percentages.Length) Debug.LogWarning($"index {index} out of bounds for Palette.myData.percentages");
            return myData.percentages[index % myData.percentages.Length];
        }

        public Color[] GetColors()
        {
            if (myData == null) this.Init();
            return myData.colors;
        }

        // given a value between 0-1, return corresponding palette color
        public Color GetColorByValue(float value, int pixelIndex = 0, float ditherThreshold = 0f)
        {
            if (myData == null) this.Init();
            Assert.AreEqual(myData.percentages.Length, myData.colors.Length, "Colors and percentages must be the same length!");
            if (value > 1) return myData.colors[myData.colors.Length - 1];
            float acc = 0f;
            float accPrev = 0f;
            for (int i = 0; i < myData.percentages.Length - 1; i++)
            {
                accPrev = acc;
                acc += myData.percentages[i];
                if (value >= acc) continue;
                float diffToPrev = (value - accPrev) * myData.percentages.Length;
                float diffToNext = (acc - value) * myData.percentages.Length;
                bool canDitherDown = ditherThreshold > diffToPrev
                    && pixelIndex % 2 == 0
                    && (diffToPrev < 0.2f || pixelIndex % 4 == 0)
                    && (diffToPrev < 0.4f || pixelIndex % 6 == 0);
                bool canDitherUp = ditherThreshold > diffToNext
                    && pixelIndex % 2 == 1
                    && i < myData.percentages.Length - 1
                    && (diffToNext < 0.2f || pixelIndex % 4 == 1)
                    && (diffToNext < 0.4f || pixelIndex % 6 == 1);

                if (canDitherDown) return i > 0 ? myData.colors[i - 1] : Color.black.toAlpha(0f);
                if (canDitherUp) return myData.colors[i + 1];
                return myData.colors[i];
            }
            return myData.colors[myData.colors.Length - 1];
        }

        public Color GetLerpedColorByValue(float value)
        {
            if (myData == null) this.Init();
            Assert.AreEqual(myData.percentages.Length, myData.colors.Length, "Colors and percentages must be the same length!");
            if (value == 1) return myData.colors[myData.colors.Length - 1];
            return Color.Lerp(
                myData.colors[GetClampedIndex(value * myData.percentages.Length)],
                myData.colors[GetClampedIndex(value * myData.percentages.Length + 1)],
                // GetColorByValue(value - 0.2f),
                // GetColorByValue(value + 0.2f),
                value * myData.percentages.Length % 1
            );
        }

        int GetClampedIndex(int index)
        {
            return Mathf.Clamp(index, 0, myData.colors.Length - 1);
        }

        int GetClampedIndex(float index)
        {
            return GetClampedIndex(Mathf.FloorToInt(index));
        }
    }
}