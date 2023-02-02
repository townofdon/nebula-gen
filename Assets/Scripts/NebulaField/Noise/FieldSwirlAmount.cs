using UnityEngine;

public class FieldSwirlAmount : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.swirlAmount;
        }
        else
        {
            return nebula2.noiseOptionsB.swirlAmount;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.swirlAmount = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.swirlAmount = incoming;
        }
        AfterChange();
    }
}
