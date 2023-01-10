using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FreeDraw
{

    public class BrushSize : MonoBehaviour
    {

        Slider slider;

        DrawingSettings settings;

        void OnEnable()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        void Awake()
        {
            settings = FindObjectOfType<DrawingSettings>();
            slider = GetComponent<Slider>();
            slider.value = Drawable.Pen_Width;
        }

        void OnValueChanged(float value)
        {
            settings.SetMarkerWidth(value);
        }
    }
}
