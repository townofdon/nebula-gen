using UnityEngine;

using CyberneticStudios.SOFramework;

public class NoiseLayerContainer : MonoBehaviour
{
    [SerializeField] NoiseLayer noiseLayer;
    [SerializeField] NoiseLayerVariable noiseLayerCurrent;
    [SerializeField] BoolVariable additionalCondition;

    void Start()
    {
        // subscribe in Start to not get the OnChange event during variable initialization
        noiseLayerCurrent.OnChanged += OnNoiseLayerChanged;
        if (additionalCondition != null) additionalCondition.OnChanged += OnAdditionalConditionChanged;
        OnNoiseLayerChanged(noiseLayerCurrent.value);
    }

    void OnDestroy()
    {
        noiseLayerCurrent.OnChanged -= OnNoiseLayerChanged;
        if (additionalCondition != null) additionalCondition.OnChanged -= OnAdditionalConditionChanged;
    }
    void OnAdditionalConditionChanged(bool _)
    {
        OnNoiseLayerChanged(noiseLayerCurrent.value);
    }

    bool AdditionalConditionsMet()
    {
        if (additionalCondition == null) return true;
        return additionalCondition.value;
    }

    void OnNoiseLayerChanged(NoiseLayer incoming)
    {
        if (noiseLayer == incoming && AdditionalConditionsMet())
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
