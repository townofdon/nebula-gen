using UnityEngine;

public class FieldDomainShiftAmount : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.domainShiftAmount;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.domainShiftAmount = incoming;
        AfterChange();
    }
}
