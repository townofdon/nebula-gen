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
    Toggle toggle;

    void OnEnable()
    {
        input?.onValueChanged.AddListener(OnValueChanged);
        slider?.onValueChanged.AddListener(OnValueChanged);
        toggle?.onValueChanged.AddListener(OnValueChanged);
        FieldEvent.OnReinitializeFields += OnReinitializeFields;
    }

    void OnDisable()
    {
        input?.onValueChanged.RemoveListener(OnValueChanged);
        slider?.onValueChanged.RemoveListener(OnValueChanged);
        toggle?.onValueChanged.RemoveListener(OnValueChanged);
        FieldEvent.OnReinitializeFields -= OnReinitializeFields;
    }

    void OnValueChanged(string incoming)
    {
        if (float.TryParse(incoming, out float result))
        {
            OnValueChanged(result);
        }
    }

    void OnValueChanged(bool incoming)
    {
        OnValueChanged(incoming ? 1 : 0);
    }

    void Awake()
    {
        FieldEvent.Init();
        nebula2 = FindObjectOfType<NebulaGen.Nebula2>();
        selectable = GetComponent<Selectable>();
        // the `as` cast will not throw an exception, but just return `null`
        input = selectable as TMP_InputField;
        slider = selectable as Slider;
        toggle = selectable as Toggle;
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (input) input.text = GetInitialValue().ToString();
        if (slider) slider.value = GetInitialValue();
        if (toggle) toggle.isOn = GetInitialValue() > 0;
    }

    void OnReinitializeFields()
    {
        Initialize();
    }

    protected void AfterChange()
    {
        nebula2.GenerateNoise();
    }

    protected abstract float GetInitialValue();

    protected abstract void OnValueChanged(float incoming);
}
