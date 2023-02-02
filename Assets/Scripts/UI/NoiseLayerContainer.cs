using UnityEngine;

using CyberneticStudios.SOFramework;
using System;

public class NoiseLayerContainer : MonoBehaviour
{
    [SerializeField] NoiseLayer noiseLayer;
    [SerializeField] NoiseLayerVariable noiseLayerCurrent;

    void Start()
    {
        // subscribe in Start to not get the OnChange event during variable initialization
        noiseLayerCurrent.OnChanged += OnNoiseLayerChanged;
        OnNoiseLayerChanged(noiseLayerCurrent.value);
    }

    void OnDestroy()
    {
        noiseLayerCurrent.OnChanged -= OnNoiseLayerChanged;
    }

    void OnNoiseLayerChanged(NoiseLayer incoming)
    {
        if (noiseLayer == incoming)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    void Activate()
    {
        gameObject.SetActive(true);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
