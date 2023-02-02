using UnityEngine;

public class FieldPerlinOffsetX : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.perlinOffset.x;
        }
        else
        {
            return nebula2.noiseOptionsB.perlinOffset.x;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.perlinOffset.x = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.perlinOffset.x = incoming;
        }
        AfterChange();
    }

    void LateUpdate()
    {
        UpdateUI();
    }
}
