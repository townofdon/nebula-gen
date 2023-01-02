using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;
using ColorPalette;


[CustomEditor(typeof(Palette))]
public class PaletteInspector : Editor
{
    const float WIDTH = 400f;

    [SerializeField] protected Texture2D plusTex;
    [SerializeField] protected Texture2D minusTex;

    protected float height = 100;

    protected bool showPalette = true;
    protected bool changeColors = false;

    protected bool adjustPCTBefore = false;
    protected float minPct = 0.01f;

    protected float paletteHeight = 100;
    protected float paletteTopMargin = 40;
    protected float paletteBotMargin = 20;

    protected float hexFieldWidth = 55;

    protected float colorChangerRowHeight = 20;
    protected float colorChangeLeftMargin = 5;
    protected float colorChangeRightMargin = 20;
    protected float colorChangeMarginBetween = 25;

    protected float buttonMarginBetween = 10;

    private Palette myPalette;


    [ExecuteInEditMode]
    public void OnEnable()
    {
        myPalette = target as Palette;
        myPalette.Init();
    }

    public override void OnInspectorGUI()
    {
        // uncomment for debugging
        //base.DrawDefaultInspector ();

        myPalette = target as Palette;

        GUILayout.Space(10);

        try
        {
            showPalette = EditorGUILayout.Foldout(showPalette, myPalette.GetPaletteData().name + " ColorPalette");

            if (showPalette)
            {
                myPalette.SetPaletteData(drawColorPalette(myPalette.GetPaletteData(), true));
            }

            changeColors = EditorGUILayout.Foldout(changeColors, " Change Colors");

            if (changeColors)
            {
                myPalette.SetPaletteData(drawColorsAndPercentages(myPalette.GetPaletteData()));
            }
        }
        catch (System.Exception e)
        {
            if (e.GetType() != typeof(ExitGUIException)) Debug.LogError(e.Message);
        }

        GUILayout.Space(25);

        myPalette.SetPaletteData(drawSizeButtons(myPalette.GetPaletteData()));

        drawActionButtons();

        GUILayout.Space(25);

        EditorUtility.SetDirty(myPalette);
    }

    protected virtual PaletteData drawColorPalette(PaletteData data, bool showHexValues = false)
    {

        // palette height silder
        //height = EditorGUILayout.Slider ("Height", height, 100, 200);

        GUILayout.Space(10);

        //paletteHeight = height;
        EditorGUILayout.BeginHorizontal();

        data.SetName(EditorGUILayout.TextField("Palette name: ", data.name));

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        Rect paletteRect;

        if (showHexValues)
        {
            paletteRect = GUILayoutUtility.GetRect(WIDTH, paletteHeight + paletteTopMargin);
        }
        else
        {
            paletteRect = GUILayoutUtility.GetRect(WIDTH, paletteHeight);
        }


        if (data.colors != null)
        {
            // show the palette
            float start = 20;

            for (int i = 0; i < data.colors.Length; i++)
            {
                Color col = data.colors[i];
                float colWidth = data.percentages[i] * (WIDTH - 35);

                //Debug.Log (i + " starts " + start + " width " + colWidth);
                float yPos = paletteRect.position.y;

                if (showHexValues)
                {
                    yPos = paletteRect.position.y + paletteTopMargin;
                }

                Rect colRect = new Rect(start, yPos, colWidth,
                     paletteHeight - paletteBotMargin);

                EditorGUIUtility.DrawColorSwatch(colRect, col);

                Rect lableRect = colRect;
                lableRect.width = 60;
                lableRect.height = 15;

                lableRect.y -= paletteTopMargin * 0.5f;

                if (i % 2 == 0)
                {
                    lableRect.y -= 15;
                }

                if (showHexValues)
                {
                    string hexString = PaletteUtils.ColorToHex(col);
                    Rect labelHexRect = new Rect(lableRect);
                    labelHexRect.width = hexFieldWidth;


                    string newHex = EditorGUI.TextField(labelHexRect, hexString);

                    if (!newHex.Equals(PaletteUtils.ColorToHex(col)))
                    {
                        data.colors[i] = PaletteUtils.HexToColor(newHex);
                    }
                }


                start += colWidth;
            }
        }

        return data;
    }

    protected virtual PaletteData drawSizeButtons(PaletteData data)
    {
        GUILayout.Space(10);

        Rect buttonRect = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Change the size of the Palette: ");

        var buttonPlus = plusTex == null ? new GUIContent("+", "Add Color") : new GUIContent(plusTex, "Add Color");
        var buttonMinus = minusTex == null ? new GUIContent("-", "Remove last Color") : new GUIContent(minusTex, "Remove last Color");

        if (GUI.Button(new Rect(WIDTH - 50 - 32 - buttonMarginBetween, buttonRect.y, 32, 32), buttonPlus, EditorStyles.miniButtonRight))
        {
            data.SetSize(data.colors.Length + 1);
        }

        if (GUI.Button(new Rect(WIDTH - 50, buttonRect.y, 32, 32), buttonMinus, EditorStyles.miniButton))
        {
            data.SetSize(data.colors.Length - 1);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        return data;
    }

    protected virtual void drawActionButtons()
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Defaults");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reverse Colors", GUILayout.Width(WIDTH / 2)))
        {
            myPalette.GetPaletteData().Reverse();
        }

        if (GUILayout.Button("Reset Percentages", GUILayout.Width(WIDTH / 2)))
        {
            myPalette.GetPaletteData().ResetPercentages();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(25);

        if (GUILayout.Button("Clear Palette", GUILayout.Width(WIDTH)))
        {
            myPalette.GetPaletteData().Clear();
        }
        if (GUILayout.Button("RESET PALETTE!!", GUILayout.Width(WIDTH)))
        {
            myPalette.GetPaletteData().ResetToDefault();
        }
    }

    protected virtual PaletteData drawColorsAndPercentages(PaletteData data)
    {
        GUILayoutUtility.GetRect(Screen.width, 10);

        adjustPCTBefore = GUILayout.Toggle(adjustPCTBefore, " adjust percentage to the left");

        Rect colorChangerRect = GUILayoutUtility.GetRect(Screen.width, data.colors.Length * colorChangerRowHeight);
        colorChangerRect.x += colorChangeLeftMargin;
        colorChangerRect.width -= colorChangeRightMargin;

        GUILayoutUtility.GetRect(Screen.width, 10);

        float startY = colorChangerRect.y + 10;

        for (int i = 0; i < data.colors.Length; i++)
        {
            // draw a little preview of the current color
            Rect colRect = new Rect(colorChangerRect.x, startY,
                                         150, colorChangerRowHeight);

            Color currentColor = data.colors[i];
            Color newColor = EditorGUI.ColorField(colRect, currentColor);

            string currentHex = PaletteUtils.ColorToHex(currentColor);

            Rect hexRect = new Rect(colorChangerRect.x + colRect.width + colorChangeMarginBetween,
                         startY, hexFieldWidth, colorChangerRowHeight);

            string newHex = EditorGUI.TextField(hexRect, currentHex);

            if (!currentHex.Equals(newHex))
            {
                data.colors[i] = PaletteUtils.HexToColor(newHex);
            }
            else if (!currentColor.ToString().Equals(newColor.ToString()))
            {
                data.colors[i] = newColor;
                data.alphas[i] = newColor.a;
            }

            float currentPct = data.percentages[i];
            float maxPct = 1.0f - (data.percentages.Length - 1) * this.minPct;
            //Debug.Log ("max % " + maxPct);

            Rect silderRect = new Rect(colorChangerRect.x + colRect.width + colorChangeMarginBetween + hexRect.width + colorChangeMarginBetween, startY,
                            colorChangerRect.width - colorChangeMarginBetween - colRect.width - colorChangeMarginBetween - hexRect.width,
                            colorChangerRowHeight);

            float newPct = EditorGUI.Slider(silderRect, currentPct, this.minPct, maxPct);
            data = adjustPct(data, i, newPct, currentPct, maxPct);


            startY += colorChangerRowHeight;
        }

        return data;
    }


    protected virtual PaletteData adjustPct(PaletteData data, int i, float newPct, float currentPct, float maxPct)
    {
        if (newPct < this.minPct)
        {
            newPct = this.minPct;
        }

        if (newPct > maxPct)
        {
            newPct = maxPct;
        }

        if (newPct != currentPct)
        {

            //if (adjustNeighborPCT (i, currentPct - newPct)) {

            data = adjustNeighborPCT(data, i, currentPct - newPct);
            data.percentages[i] = newPct;

            // changes been made to neighbors, check if totalPcts is still 1 -> 100%
            float totalPcts = data.GetTotalPct();

            if (totalPcts >= 1f)
            {
                float rounding = totalPcts - 1f;
                if (rounding > 0)
                {
                    //Debug.Log ("rounding " + newPct + " minus " + rounding + " to " + (newPct - rounding));
                    // always cut the last for the rounding!
                    data.percentages[data.percentages.Length - 1] -= rounding;
                }
                else if (rounding < 0)
                {
                    data.percentages[data.percentages.Length - 1] += rounding;
                }
            }

        }

        return data;
    }

    /// <summary>
    /// Adjusts the neighbor Percentage, returns false if the neighbor value woulde be under this.minPct!.
    /// </summary>
    /// <returns><c>true</c>, if neighbor PC was adjusted, <c>false</c> otherwise.</returns>
    /// <param name="i">The index.</param>
    /// <param name="pctDiff">Pct diff.</param>
    protected virtual PaletteData adjustNeighborPCT(PaletteData data, int i, float pctDiff)
    {
        int neiborIndex = i;

        if (adjustPCTBefore)
        {
            if (i - 1 >= 0)
            {
                neiborIndex = i - 1;
            }
            else
            {
                neiborIndex = data.percentages.Length - 1;
            }
        }
        else
        {
            if (i + 1 <= data.percentages.Length - 1)
            {
                neiborIndex = i + 1;
            }
            else
            {
                neiborIndex = 0;
            }
        }

        data.percentages[neiborIndex] += pctDiff;
        float newNeighborValue = data.percentages[neiborIndex];

        if (newNeighborValue < this.minPct)
        {
            data.percentages[neiborIndex] = this.minPct;
            data = adjustNeighborPCT(data, neiborIndex, newNeighborValue - this.minPct);
        }

        return data;
    }



}