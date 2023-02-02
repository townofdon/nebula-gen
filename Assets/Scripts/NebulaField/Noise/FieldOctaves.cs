using UnityEngine;

public class FieldOctaves : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.octaves;
        }
        else
        {
            return nebula2.noiseOptionsB.octaves;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.octaves = (int)incoming;
        }
        else
        {
            nebula2.noiseOptionsB.octaves = (int)incoming;
        }
        AfterChange();
    }
}
