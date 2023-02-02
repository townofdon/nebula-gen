using UnityEngine;
using UnityEngine.UI;

using CyberneticStudios.SOFramework;
using System;

[RequireComponent(typeof(Toggle))]
public class NoiseLayerSelect : MonoBehaviour
{
    [SerializeField] NoiseLayer noiseLayer;
    [SerializeField] NoiseLayerVariable noiseLayerCurrent;

    Toggle toggle;

    void Awake()
    {
        noiseLayerCurrent.ResetVariable();
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        noiseLayerCurrent.OnChanged += OnNoiseLayerChanged;
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnValueChanged);
        noiseLayerCurrent.OnChanged -= OnNoiseLayerChanged;
    }

    void Start()
    {
        toggle.SetIsOnWithoutNotify(noiseLayer == noiseLayerCurrent.value);
    }

    void OnValueChanged(bool isOn)
    {
        if (isOn && noiseLayer == noiseLayerCurrent.value) return;
        noiseLayerCurrent.value = noiseLayer;
        toggle.SetIsOnWithoutNotify(true);
    }

    void OnNoiseLayerChanged(NoiseLayer incoming)
    {
        toggle.SetIsOnWithoutNotify(noiseLayer == incoming);
    }
}
