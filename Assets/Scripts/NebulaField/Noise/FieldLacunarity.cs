using UnityEngine;

public class FieldLacunarity : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.lacunarity;
        }
        else
        {
            return nebula2.noiseOptionsB.lacunarity;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.lacunarity = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.lacunarity = incoming;
        }
        AfterChange();
    }
}
