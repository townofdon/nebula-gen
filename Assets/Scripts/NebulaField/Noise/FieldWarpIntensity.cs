using UnityEngine;

public class FieldWarpIntensity : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.warpIntensity;
        }
        else
        {
            return nebula2.noiseOptionsB.warpIntensity;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.warpIntensity = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.warpIntensity = incoming;
        }
        AfterChange();
    }
}
