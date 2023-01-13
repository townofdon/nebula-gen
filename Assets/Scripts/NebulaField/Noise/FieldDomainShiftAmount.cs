using UnityEngine;

public class FieldDomainShiftAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.domainShiftAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.domainShiftAmount = incoming;
        AfterChange();
    }
}
