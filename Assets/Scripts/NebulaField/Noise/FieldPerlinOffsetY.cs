using UnityEngine;

public class FieldPerlinOffsetY : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.perlinOffset.y;
        }
        else
        {
            return nebula2.noiseOptionsB.perlinOffset.y;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.perlinOffset.y = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.perlinOffset.y = incoming;
        }
        AfterChange();
    }
}
