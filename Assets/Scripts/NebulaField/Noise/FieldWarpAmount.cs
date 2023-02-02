using UnityEngine;

public class FieldWarpAmount : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.warpAmount;
        }
        else
        {
            return nebula2.noiseOptionsB.warpAmount;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.warpAmount = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.warpAmount = incoming;
        }
        AfterChange();
    }
}
