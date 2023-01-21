using UnityEngine;

public class FieldDomainShiftPasses : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseOptions.domainShiftPasses;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseOptions.domainShiftPasses = (int)Mathf.Clamp(incoming, 1, 3);
        AfterChange();
    }
}
