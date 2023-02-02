using UnityEngine;

public class FieldSwirlIntensity : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.swirlIntensity;
        }
        else
        {
            return nebula2.noiseOptionsB.swirlIntensity;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.swirlIntensity = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.swirlIntensity = incoming;
        }
        AfterChange();
    }
}
