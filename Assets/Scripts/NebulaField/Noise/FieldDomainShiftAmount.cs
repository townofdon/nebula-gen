using UnityEngine;

public class FieldDomainShiftAmount : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.domainShiftAmount;
        }
        else
        {
            return nebula2.noiseOptionsB.domainShiftAmount;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.domainShiftAmount = incoming;
        }
        else
        {
            nebula2.noiseOptionsB.domainShiftAmount = incoming;
        }
        AfterChange();
    }
}
