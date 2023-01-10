using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Selectable))]
public abstract class FieldBase : MonoBehaviour
{
    protected NebulaGen.Nebula2 nebula2;

    Selectable selectable;
    TMP_InputField input;
    Slider slider;

    void OnEnable()
    {
        input?.onValueChanged.AddListener(OnValueChanged);
        slider?.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {
        input?.onValueChanged.RemoveListener(OnValueChanged);
        slider?.onValueChanged.RemoveListener(OnValueChanged);
    }

    void OnValueChanged(string incoming)
    {
        if (float.TryParse(incoming, out float result))
        {
            OnValueChanged(result);
        }
    }

    void Awake()
    {
        nebula2 = FindObjectOfType<NebulaGen.Nebula2>();
        selectable = GetComponent<Selectable>();
        // the `as` cast will not throw an exception, but just return `null`
        input = selectable as TMP_InputField;
        slider = selectable as Slider;
    }

    void Start()
    {
        if (input) input.text = GetInitialValue().ToString();
        if (slider) slider.value = GetInitialValue();
    }

    protected void AfterChange()
    {
        nebula2.GenerateNoise();
    }

    protected abstract float GetInitialValue();

    protected abstract void OnValueChanged(float incoming);
}
