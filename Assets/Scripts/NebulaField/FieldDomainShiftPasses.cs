using UnityEngine;

public class FieldDomainShiftPasses : FieldBase
{
    protected override float GetInitialValue()
    {
        return nebula2.noiseLayerA.domainShiftPasses;
    }

    protected override void OnValueChanged(float incoming)
    {
        nebula2.noiseLayerA.domainShiftPasses = (int)Mathf.Clamp(incoming, 1, 3);
        AfterChange();
    }
}
