using UnityEngine;

public class FieldDomainShiftPasses : NoiseLayerFieldBase
{
    protected override float GetInitialValue()
    {
        if (noiseLayer == NoiseLayer.A)
        {
            return nebula2.noiseOptionsA.domainShiftPasses;
        }
        else
        {
            return nebula2.noiseOptionsB.domainShiftPasses;
        }
    }

    protected override void OnValueChanged(float incoming)
    {
        if (noiseLayer == NoiseLayer.A)
        {
            nebula2.noiseOptionsA.domainShiftPasses = (int)Mathf.Clamp(incoming, 0, 2);
        }
        else
        {
            nebula2.noiseOptionsB.domainShiftPasses = (int)Mathf.Clamp(incoming, 0, 2);
        }
        AfterChange();
    }
}
