using System;
using UnityEngine;
using SimpleJSON;

namespace ColorPalette
{

    [Serializable]
    public class PaletteData : System.Object
    {
        public static readonly string namePrefix = "PaletteData";

        // public PaletteData(string name = null)
        // {
        //     if (string.IsNullOrEmpty(name))
        //     {
        //         this._name = name;
        //     }
        //     else
        //     {
        //         this._name = namePrefix + "_" + name;
        //     }

        //     this._colors = getDefaultColors();
        //     this._alphas = getDefaultAlphas();
        //     this._percentages = getDefaultPercentages();
        //     this._totalWidth = 0;
        // }

        [SerializeField] string _name;
        [SerializeField] Color[] _colors;
        [SerializeField] float[] _alphas;
        [SerializeField] float[] _percentages;
        [SerializeField] float _totalWidth;

        public string name => _name;
        public Color[] colors => _colors;
        public float[] alphas => _alphas;
        public float[] percentages => _percentages;
        public float totalWidth => _totalWidth;


        #region publicMethods

        public void Clear()
        {
            this._colors = new Color[] { Color.black };
            this._alphas = new float[] { 1f };
            this._percentages = new float[] { 1f };
            this._totalWidth = 0;
        }

        public void ResetToDefault()
        {
            if (string.IsNullOrEmpty(name))
            {
                this._name = "";
            }
            this._colors = GetDefaultColors();
            this._alphas = GetDefaultAlphas();
            this._percentages = GetDefaultPercentages();
            this._totalWidth = 0;
        }

        public void ResetPercentages()
        {
            for (int i = 0; i < this.percentages.Length; i++)
            {
                this.percentages[i] = 1f / this.percentages.Length;
            }
        }

        public void Reverse()
        {
            Color[] temp_colors = new Color[this.colors.Length];
            float[] temp_alphas = new float[this.alphas.Length];
            float[] temp_percentages = new float[this.percentages.Length];
            int lastIndex = colors.Length - 1;
            for (int i = 0; i < temp_colors.Length; i++)
            {
                temp_colors[i] = colors[lastIndex - i];
                temp_alphas[i] = alphas[lastIndex - i];
                temp_percentages[i] = percentages[lastIndex - i];
            }
            _colors = temp_colors;
            _alphas = temp_alphas;
            _percentages = temp_percentages;
        }

        public JSONClass GetJsonPalette()
        {
            JSONClass jClass = new JSONClass();

            jClass["name"] = name;

            string[] hexArray = PaletteUtils.getHexArrayFromColors(this.colors);

            for (int i = 0; i < hexArray.Length; i++)
            {
                jClass["colors"][i] = hexArray[i];
            }

            for (int i = 0; i < this.alphas.Length; i++)
            {
                jClass["alphas"][i].AsFloat = this.colors[i].a;
            }

            for (int i = 0; i < this.percentages.Length; i++)
            {
                jClass["percentages"][i].AsFloat = this.percentages[i];
            }

            jClass["totalWidth"].AsFloat = this._totalWidth;

            //Debug.Log ("name: " + this.name + " " + jClass ["colors"].ToString () + " " + jClass ["percentages"].ToString ());

            return jClass;
        }


        public void SetPalette(JSONClass jClass)
        {
            int size = jClass["colors"].Count;

            this._name = jClass["name"];

            string[] hexArray = new string[size];

            for (int i = 0; i < size; i++)
            {
                hexArray[i] = jClass["colors"][i];
            }

            this._colors = PaletteUtils.getColorsArrayFromHex(hexArray);


            size = jClass["alphas"].Count;

            // if the size of the file is different than the standard size -> init()
            if (this.alphas.Length != size)
            {
                this._alphas = new float[size];
            }

            for (int i = 0; i < size; i++)
            {
                float alphaValue = jClass["alphas"][i].AsFloat;
                this.alphas[i] = alphaValue;
                this.colors[i].a = alphaValue;
            }

            size = jClass["percentages"].Count;

            // if the size of the file is different than the standard size -> init()
            if (this.percentages.Length != size)
            {
                this._percentages = new float[size];
            }

            for (int i = 0; i < size; i++)
            {
                this.percentages[i] = jClass["percentages"][i].AsFloat;
            }

            this._totalWidth = jClass["totalWidth"].AsFloat;
        }


        /// <summary>
        /// changes the size of the colors and the other arrays. Make sure to initialize it first!
        /// </summary>
        /// <returns><c>true</c>, if size was changed, <c>false</c> otherwise.</returns>
        /// <param name="newSize">New size.</param>
        public bool SetSize(int newSize)
        {
            if (newSize != this.colors.Length)
            {
                if (newSize == 0)
                {
                    return false;
                }

                if (newSize > this.colors.Length)
                {
                    if (newSize == 1)
                    {
                        Clear();
                        return true;
                    }

                    Color[] newColors = new Color[newSize];
                    this.colors.CopyTo(newColors, 0);
                    this._colors = newColors;

                    float[] newAlphas = new float[newSize];
                    this.alphas.CopyTo(newAlphas, 0);
                    this._alphas = newAlphas;

                    float[] newPercentages = new float[newSize];
                    this.percentages.CopyTo(newPercentages, 0);
                    this._percentages = newPercentages;
                    float newPercentage = 1f / this._percentages.Length;
                    this._percentages[this._percentages.Length - 1] = newPercentage;
                    for (int i = 0; i < this._percentages.Length - 1; i++)
                    {
                        this._percentages[i] = (this._percentages[i] * (this._percentages.Length - 1) / (this._percentages.Length));
                    }

                    if (this.colors.Length > 1)
                    {
                        this.colors[this.colors.Length - 1] = this.colors[this.colors.Length - 2];
                        this.alphas[this.alphas.Length - 1] = this.alphas[this.alphas.Length - 2];
                    }

                    // when adding a new Color the % will adjust automaticlly due to the 
                    // inspector script

                    return true;
                }
                else
                {
                    int sizeDiff = this.colors.Length - newSize;

                    Color[] newColors = new Color[newSize];
                    float[] newAlphas = new float[newSize];
                    float[] newPercentages = new float[newSize];

                    for (int i = 0; i < newColors.Length; i++)
                    {
                        newColors[i] = this.colors[i];
                        newAlphas[i] = this.colors[i].a;
                        newPercentages[i] = this.percentages[i];
                    }
                    this._colors = newColors;
                    this._alphas = newAlphas;
                    this._percentages = newPercentages;

                    // when removing though, the last value will be streched
                    FillUpLastPercentage(sizeDiff);

                    return true;
                }
            }

            return false;
        }

        public float GetTotalPct()
        {
            float total = 0;
            foreach (float pct in this.percentages)
            {
                total += pct;
            }
            return total;
        }

        public void SetName(string name)
        {
            this._name = name;
        }

        #endregion

        protected void FillUpLastPercentage(int sizeDifference)
        {
            if (this.percentages.Length == 0) return;
            float currentTotal = GetTotalPct();
            if (currentTotal < 1)
            {
                this.percentages[this.percentages.Length - 1] += (1 - currentTotal);
            }
        }


        #region staticMethods

        // public static PaletteData getInstance(JSONClass jClass)
        // {
        //     PaletteData palette = new PaletteData();
        //     //Debug.Log ("getInstance: " + jClass.ToString ());
        //     palette.SetPalette(jClass);
        //     return palette;
        // }

        public static Color[] GetDefaultColors()
        {
            return PaletteUtils.getColorsArrayFromHex(new string[] { "69D2E7", "A7DBD8", "E0E4CC", "F38630", "FA6900" });
        }

        public static float[] GetDefaultAlphas()
        {
            return new float[] { 1f, 1f, 1f, 1f, 1f };
        }

        public static float[] GetDefaultPercentages()
        {
            return new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
        }

        #endregion

        public override string ToString()
        {
            JSONClass jClass = GetJsonPalette();
            return "[" + this.GetType() + "] name: " + this.name + " colors: " + jClass["colors"].ToString()
                    + " alphas: " + jClass["alphas"].ToString()
                    + " percentages: " + jClass["percentages"].ToString();
        }

    }
}

